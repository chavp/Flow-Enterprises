using Flowenter.Domain.Models;
using Microsoft.Extensions.Options;

namespace Flowenter.Api.Services;

public sealed class TenantProvider : ITenantProvider
{
    public const string TenantIdHeaderName = "Flowenter-TenantId";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TenantPartiesConnectionStrings _tenantPartiesConnectionStrings;

    public TenantProvider(IHttpContextAccessor httpContextAccessor,
        IOptions<TenantPartiesConnectionStrings> tenantPartiesConnectionStrings)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantPartiesConnectionStrings = tenantPartiesConnectionStrings.Value;
    }

    public string GetTenantConnectionString()
    {
        if (!_tenantPartiesConnectionStrings.Values.ContainsKey(GetTenantId().Value.ToString()))
        {
            throw new ApplicationException("Tenant Connection String is missing.");
        }

        return _tenantPartiesConnectionStrings.Values[GetTenantId().Value.ToString()];
    }

    public Guid? GetTenantId()
    {
        var httpContext = _httpContextAccessor.HttpContext?
            .Request
            .Headers[TenantIdHeaderName];

        if (!httpContext.HasValue ||
            !Guid.TryParse(httpContext.Value, out var tenantId) ||
            !Tenants.All.Contains(tenantId))
        {
            throw new ApplicationException("Tenant ID header is missing.");
        }

        return tenantId;
    }
}
