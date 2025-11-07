using Flowenter.Parties.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Extensions;

public static class DbContextExtensions
{
    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        var partiesConnection = builder.Configuration.GetConnectionString("PartiesConnection")
            ?? throw new InvalidOperationException("Connection string 'PartiesConnection' not found.");

        builder.Services.AddSingleton<IDbContextFactory<PartiesContext>, PartiesDbContextFactory>(
                x => new PartiesDbContextFactory(partiesConnection)
        );

        return builder;
    }
}
