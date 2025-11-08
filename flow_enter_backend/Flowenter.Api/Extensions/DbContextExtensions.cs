using Flowenter.Domain.Models;
using Flowenter.Parties.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Flowenter.Api.Extensions;

public static class DbContextExtensions
{
    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IDbContextFactory<PartiesContext>, PartiesDbContextFactory>();

        return builder;
    }
}
