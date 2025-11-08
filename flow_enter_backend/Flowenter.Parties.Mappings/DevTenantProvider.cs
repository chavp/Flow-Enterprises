using Flowenter.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.Mappings;

public sealed class DevTenantProvider : ITenantProvider
{
    public Guid? GetTenantId()
    {
        return Guid.Empty;
    }

    public string GetTenantConnectionString()
    {
        return "Host=localhost;Database=flowenter-dev;Username=admin;Password=admin123;Port=5432";
    }
}
