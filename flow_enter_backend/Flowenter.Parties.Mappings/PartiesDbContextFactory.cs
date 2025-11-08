using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.Mappings;

public class PartiesDbContextFactory : IDbContextFactory<PartiesContext>
{
    private readonly ITenantProvider _tenantProvider;

    public PartiesDbContextFactory(ITenantProvider tenantProvider)
    {
        _tenantProvider = tenantProvider;
    }

    public PartiesContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<PartiesContext>()
            .UseNpgsql(_tenantProvider.GetTenantConnectionString())
            .Options;

        return new PartiesContext(options, _tenantProvider);
    }
}
