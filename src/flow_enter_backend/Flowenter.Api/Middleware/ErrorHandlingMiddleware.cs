using FlowShops.Core.Exceptions;
using Serilog;
using System.Net;
using System.Text.Json;
using ValidationException = FlowShops.Core.Exceptions.ValidationException;

namespace Flowenter.Api.Middleware;

/// <summary>
/// Global error handling middleware that catches and processes exceptions
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _environment;

    public ErrorHandlingMiddleware(RequestDelegate next, IWebHostEnvironment environment)
    {
        _next = next;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        Log.Error(exception, "An error occurred: {Message}", exception.Message);

        var (statusCode, message, errors) = GetErrorDetails(exception);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Message = message,
            Errors = errors,
            TraceId = context.TraceIdentifier
        };

        // Only include stack trace in development environment
        if (_environment.IsDevelopment())
        {
            response.StackTrace = exception.StackTrace;
            response.Details = exception.ToString();
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }

    private (HttpStatusCode statusCode, string message, List<string>? errors) GetErrorDetails(Exception exception)
    {
        return exception switch
        {
            NotFoundException notFound =>
                (HttpStatusCode.NotFound, notFound.Message, null),

            BusinessException business =>
                (HttpStatusCode.BadRequest, business.Message, null),

            ValidationException validation =>
                (HttpStatusCode.BadRequest, "Validation failed", validation.Errors?.ToList()),

            UnauthorizedAccessException _ =>
                (HttpStatusCode.Unauthorized, "Unauthorized access", null),

            InvalidOperationException invalid =>
                (HttpStatusCode.BadRequest, invalid.Message, null),

            ArgumentException argument =>
                (HttpStatusCode.BadRequest, argument.Message, null),

            _ => (HttpStatusCode.InternalServerError,
                  "An error occurred while processing your request",
                  null)
        };
    }
}

/// <summary>
/// Error response model
/// </summary>
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string>? Errors { get; set; }
    public string? TraceId { get; set; }
    public string? StackTrace { get; set; }
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
