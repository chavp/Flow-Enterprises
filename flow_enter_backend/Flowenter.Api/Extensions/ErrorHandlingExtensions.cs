using Flowenter.Api.Middleware;

namespace Flowenter.Api.Extensions;

public static class ErrorHandlingExtensions
{
    /// <summary>
    /// Adds global error handling middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}