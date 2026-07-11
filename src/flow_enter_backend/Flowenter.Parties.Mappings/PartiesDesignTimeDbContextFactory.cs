using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.Mappings;

public class PartiesDesignTimeDbContextFactory : IDesignTimeDbContextFactory<PartiesContext>
{
    public PartiesContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PartiesContext>();
        optionsBuilder.UseNpgsql("Server=localhost;TrustServerCertificate=True;Database=flowenter-dev;User Id=admin;Password=admin123");
        return new PartiesContext(optionsBuilder.Options, new DevTenantProvider());
    }
}
