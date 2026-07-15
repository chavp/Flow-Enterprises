using Flowenter.Domain.Mappings;
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
        optionsBuilder.UseSqlServer("Server=localhost;TrustServerCertificate=True;Database=flow-enter-parties;User Id=sa;Password=Admin@1234");
        return new PartiesContext(optionsBuilder.Options, new DevTenantProvider());
    }
}
