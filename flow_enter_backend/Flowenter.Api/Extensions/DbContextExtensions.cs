using Flowenter.Parties.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Api.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var partiesConnection = configuration.GetConnectionString("PartiesConnection")
            ?? throw new InvalidOperationException("Connection string 'PartiesConnection' not found.");

        services.AddSingleton<IDbContextFactory<PartiesContext>, PartiesDbContextFactory>(
                x => new PartiesDbContextFactory(partiesConnection)
        );

        return services;
    }
}
