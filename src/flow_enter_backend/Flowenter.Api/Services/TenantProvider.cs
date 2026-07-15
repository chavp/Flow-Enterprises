using Flowenter.Domain.Models;
using Microsoft.Extensions.Options;

namespace Flowenter.Api.Services;

public sealed class TenantProvider : ITenantProvider
{
    public const string PartiesTenantIdHeaderName = "Flowenter-Parties-TenantId";
    public const string ProducsTenantIdHeaderName = "Flowenter-Products-TenantId";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TenantPartiesConnectionStrings _tenantPartiesConnectionStrings;
    private readonly TenantProductsConnectionStrings _tenantProductsConnectionStrings;

    public TenantProvider(IHttpContextAccessor httpContextAccessor,
        IOptions<TenantPartiesConnectionStrings> tenantPartiesConnectionStrings,
        IOptions<TenantProductsConnectionStrings> tenantProductsConnectionStrings)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantPartiesConnectionStrings = tenantPartiesConnectionStrings.Value;
        _tenantProductsConnectionStrings = tenantProductsConnectionStrings.Value;
    }

    public string GetPartiesTenantConnectionString()
    {
        if (!_tenantPartiesConnectionStrings.Values.ContainsKey(GetPartiesTenantId().Value.ToString()))
        {
            throw new ApplicationException("Tenant Connection String is missing.");
        }

        return _tenantPartiesConnectionStrings.Values[GetPartiesTenantId().Value.ToString()];
    }

    public string GetProductsTenantConnectionString()
    {
        if (!_tenantProductsConnectionStrings.Values.ContainsKey(GetProductsTenantId().Value.ToString()))
        {
            throw new ApplicationException("Tenant Connection String is missing.");
        }

        return _tenantProductsConnectionStrings.Values[GetProductsTenantId().Value.ToString()];
    }

    public Guid? GetPartiesTenantId()
    {
        var httpContext = _httpContextAccessor.HttpContext?
            .Request
            .Headers[PartiesTenantIdHeaderName];

        if (!httpContext.HasValue ||
            !Guid.TryParse(httpContext.Value, out var tenantId) ||
            !Tenants.All.Contains(tenantId))
        {
            throw new ApplicationException("Tenant ID header is missing.");
        }

        return tenantId;
    }

    public Guid? GetProductsTenantId()
    {
        var httpContext = _httpContextAccessor.HttpContext?
            .Request
            .Headers[ProducsTenantIdHeaderName];

        if (!httpContext.HasValue ||
            !Guid.TryParse(httpContext.Value, out var tenantId) ||
            !Tenants.All.Contains(tenantId))
        {
            throw new ApplicationException("Tenant ID header is missing.");
        }

        return tenantId;
    }
}
