using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.Mappings;

public class PartiesDbContextFactory : IDbContextFactory<PartiesContext>
{
    private DbContextOptions<PartiesContext> _options;
    public PartiesDbContextFactory(string connectionString)
    {
        _options = new DbContextOptionsBuilder<PartiesContext>()
            .UseNpgsql(connectionString)
            .Options;
    }

    public PartiesContext CreateDbContext()
    {
        return new PartiesContext(_options);
    }
}
