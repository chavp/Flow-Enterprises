using Flowenter.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.Mappings;

public sealed class DevTenantProvider : ITenantProvider
{
    public Guid? GetPartiesTenantId()
    {
        return Guid.Empty;
    }

    public string GetPartiesTenantConnectionString()
    {
        return "Host=localhost;Database=flowenter-dev;Username=admin;Password=admin123;Port=5432";
    }
}
