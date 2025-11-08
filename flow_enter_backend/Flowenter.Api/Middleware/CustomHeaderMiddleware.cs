using Flowenter.Api.Services;

namespace Flowenter.Api.Middleware;

public class CustomHeaderMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _env;

    public CustomHeaderMiddleware(RequestDelegate next, IHostEnvironment env)
    {
        _next = next;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (_env.IsDevelopment())
        {
            context.Request.Headers.Add(TenantProvider.TenantIdHeaderName, Guid.Empty.ToString());
        }
        await _next(context);
    }
}
