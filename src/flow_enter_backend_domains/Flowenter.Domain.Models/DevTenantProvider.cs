using Flowenter.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Domain.Mappings;

public sealed class DevTenantProvider : ITenantProvider
{
    public Guid? GetPartiesTenantId()
    {
        return Guid.Empty;
    }

    public string GetPartiesTenantConnectionString()
    {
        return "Server=localhost;TrustServerCertificate=True;Database=flow-enter;User Id=sa;Password=Admin@1234";
    }

    public string GetProductsTenantConnectionString()
    {
        return "Server=localhost;TrustServerCertificate=True;Database=flow-enter;User Id=sa;Password=Admin@1234";
    }
}
