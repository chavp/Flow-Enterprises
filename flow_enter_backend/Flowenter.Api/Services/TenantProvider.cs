using Flowenter.Domain.Models;
using Microsoft.Extensions.Options;

namespace Flowenter.Api.Services;

public sealed class TenantProvider : ITenantProvider
{
    public const string PartiesTenantIdHeaderName = "Flowenter-Parties-TenantId";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TenantPartiesConnectionStrings _tenantPartiesConnectionStrings;

    public TenantProvider(IHttpContextAccessor httpContextAccessor,
        IOptions<TenantPartiesConnectionStrings> tenantPartiesConnectionStrings)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantPartiesConnectionStrings = tenantPartiesConnectionStrings.Value;
    }

    public string GetPartiesTenantConnectionString()
    {
        if (!_tenantPartiesConnectionStrings.Values.ContainsKey(GetPartiesTenantId().Value.ToString()))
        {
            throw new ApplicationException("Tenant Connection String is missing.");
        }

        return _tenantPartiesConnectionStrings.Values[GetPartiesTenantId().Value.ToString()];
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
}
