using Flowenter.Parties.Mappings;
using Flowenter.Parties.Models.GeographicBoundaryModels;
using Flowenter.Parties.Models.PartyModels;
using Microsoft.EntityFrameworkCore;

namespace Parties.Tests;

public class TestParties
{
    [Fact]
    public async Task TestLanguages()
    {
        var options = new DbContextOptionsBuilder<PartiesContext>()
            .UseSqlServer(
                "Server=localhost;TrustServerCertificate=True;Database=flow-enter;User Id=sa;Password=Admin@1234;MultipleActiveResultSets=True")
            .Options;

        await using var context = new PartiesContext(options, new DevTenantProvider());
        await context.Database.MigrateAsync();

        await context.Languages.AddAsync(new Language
        {
            Id = Guid.NewGuid(),
            Code = Language.EN,
            Description = "English",
            CreatedBy = "test"
        });
        await context.Languages.AddAsync(new Language
        {
            Id = Guid.NewGuid(),
            Code = Language.TH,
            Description = "ภาษาไทย",
            CreatedBy = "test"
        });

        await context.SaveChangesAsync();

        var savedLanguages = await context.Languages
            .OrderBy(x => x.Code)
            .ToListAsync();

        Assert.Equal(2, savedLanguages.Count);
        Assert.Equal(new[] { Language.EN, Language.TH }, savedLanguages.Select(x => x.Code).ToArray());
    }

    [Fact]
    public void TestGeographicBoundaryTypes()
    {
        var options = new DbContextOptionsBuilder<PartiesContext>()
            .UseSqlServer(
                "Server=localhost;TrustServerCertificate=True;Database=flow-enter;User Id=sa;Password=Admin@1234;MultipleActiveResultSets=True")
            .Options;

        using var context = new PartiesContext(options, new DevTenantProvider());

        context.Add(new GeographicBoundaryType
        {
            Id = Guid.NewGuid(),
            Code = GeographicBoundaryType.Country,
            Name = "ประเทศ",
            CreatedBy = "test"
        });

        context.SaveChanges();
    }
}
