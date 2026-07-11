using Flowenter.Domain.Models;
using Flowenter.Parties.Mappings.Extensions;
using Flowenter.Parties.Models.ContactMechanismModels;
using Flowenter.Parties.Models.GeographicBoundaryModels;
using Flowenter.Parties.Models.PartyModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Flowenter.Parties.Mappings;

public sealed class PartiesContext : DbContext
{
    // PartyModels
    public DbSet<Language> Languages => Set<Language>();

    public DbSet<PartyType> PartyTypes => Set<PartyType>();
    public DbSet<PartyRoleType> PartyRoleTypes => Set<PartyRoleType>();
    public DbSet<LegalStructure> LegalStructures => Set<LegalStructure>();
    public DbSet<PartyCategory> PartyCategories => Set<PartyCategory>();

    public DbSet<Party> Parties => Set<Party>();
    public DbSet<PartyRole> PartyRoles => Set<PartyRole>();
    public DbSet<PartyContactMechanism> PartyContactMechanisms => Set<PartyContactMechanism>();
    public DbSet<PartyClassification> PartyClassifications => Set<PartyClassification>();
    public DbSet<PersonName> PersonNames => Set<PersonName>();

    // GeographicBoundaryModels
    public DbSet<GeographicBoundaryType> GeographicBoundaryTypes => Set<GeographicBoundaryType>();
    public DbSet<GeographicBoundary> GeographicBoundaries => Set<GeographicBoundary>();
    public DbSet<Country> Countries => Set<Country>();

    // ContactMechanismModels
    public DbSet<ContactMechanismType> ContactMechanismTypes => Set<ContactMechanismType>();
    public DbSet<ContactMechanism> ContactMechanisms => Set<ContactMechanism>();

    // FinancialAccountModels
    public DbSet<FinancialAccount> FinancialAccounts => Set<FinancialAccount>();

    public readonly ITenantProvider _tenantProvider;

    public PartiesContext(DbContextOptions<PartiesContext> options,
        ITenantProvider tenantProvider) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("parties");

        modelBuilder.Entity<Party>(builder =>
        {
            builder.HasQueryFilter(pr => pr.TenantId == _tenantProvider.GetPartiesTenantId());
        });
        modelBuilder.Entity<Person>();
        modelBuilder.Entity<Organization>();

        modelBuilder.Entity<PartyRole>(builder =>
        {
            builder.HasQueryFilter(pr => pr.TenantId == _tenantProvider.GetPartiesTenantId());
        });
        modelBuilder.Entity<Enterprise>();
        modelBuilder.Entity<Customer>();

        modelBuilder.Entity<ContactMechanism>(builder =>
        {
            builder.HasQueryFilter(cm => cm.TenantId == _tenantProvider.GetPartiesTenantId());
        });
        modelBuilder.Entity<ElectronicAddress>();
        modelBuilder.Entity<PostalAddress>();
        modelBuilder.Entity<TelecommunicationNumber>();

        modelBuilder.Entity<GeographicBoundary>();
        modelBuilder.Entity<Country>();

        modelBuilder.ApplyUpperConverter(["Code", "Number"]);

        modelBuilder.Entity<Party>()
            .HasMany(e => e.FinancialAccounts)
            .WithOne(e => e.Owner)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Party>()
            .HasMany(e => e.ContactMechanisms)
            .WithMany(e => e.Parties)
            .UsingEntity<PartyContactMechanism>(
                r => r.HasOne(e => e.ContactMechanism).WithMany(),
                l => l.HasOne(e => e.Party).WithMany()
            );

        seeds(modelBuilder);
    }

    private void seeds(ModelBuilder modelBuilder) {

    //    modelBuilder.Entity<Language>().HasData([
    //new PartyType
    //        {
    //            Id = Guid.NewGuid(),
    //            Code = Language.TH,
    //            Name = "ภาษาไทย",
    //            CreatedBy = "seed",
    //            Revision = 0
    //        },
    //        new PartyType
    //        {
    //            Id = Guid.NewGuid(),
    //            Code = Language.EN,
    //            Name = "English",
    //            CreatedBy = "seed",
    //            Revision = 0
    //        }
    //    ]);

        modelBuilder.Entity<PartyType>().HasData([
            new PartyType
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Code = PartyType.Person,
                Name = "Person",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyType
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Code = PartyType.Organization,
                Name = "Organization",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            }
        ]);

        modelBuilder.Entity<PartyRoleType>().HasData([
            new PartyRoleType
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Code = PartyRoleType.Enterprise,
                Name = "Enterprise",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRoleType
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Code = PartyRoleType.Customer,
                Name = "Customer",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            }
        ]);

        modelBuilder.Entity<LegalStructure>().HasData([
            new LegalStructure
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Code = LegalStructure.SoleProprietorship,
                Name = "กิจการเจ้าของคนเดียว",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new LegalStructure
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Code = LegalStructure.Partnership,
                Name = "ห้างหุ้นส่วน",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new LegalStructure
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Code = LegalStructure.Corporation,
                Name = "บริษัทมหาชน (หรือบริษัทจำกัดขนาดใหญ่)",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new LegalStructure
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Code = LegalStructure.LimitedLiabilityCompany,
                Name = "บริษัทจำกัด",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            }
        ]);

        modelBuilder.Entity<GeographicBoundaryType>().HasData([
            new GeographicBoundaryType
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                Code = GeographicBoundaryType.Country,
                Name = "Country",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            }
        ]);
    }

    private async Task saveChangeAsync(CancellationToken cancellationToken = default)
    {
        updateAuditableEntities(DateTime.UtcNow);
    }

    public override int SaveChanges()
    {
        saveChangeAsync().Wait();

        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await saveChangeAsync(cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void updateAuditableEntities(DateTime utcNow)
    {
        foreach (EntityEntry<BaseEntity> entityEntry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(nameof(BaseEntity.CreatedAtUtc)).CurrentValue = utcNow;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(nameof(BaseEntity.UpdatedAtUtc)).CurrentValue = utcNow;

                var revCurrentValue = entityEntry.Property(nameof(BaseEntity.Revision)).CurrentValue;
                entityEntry.Property(nameof(BaseEntity.Revision)).CurrentValue = Convert.ToUInt64(revCurrentValue) + 1;

                if (entityEntry.Property(nameof(BaseEntity.UpdatedBy)).CurrentValue == null)
                {
                    //entityEntry.Property(nameof(BaseEntity.CreatedBy)).CurrentValue = GetCurrentUserId();
                    entityEntry.Property(nameof(BaseEntity.UpdatedBy)).CurrentValue = Environment.MachineName;
                }
            }
        }

        foreach (EntityEntry<ITenantEnabled> entityEntry in ChangeTracker.Entries<ITenantEnabled>())
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property("TenantId").CurrentValue = _tenantProvider.GetPartiesTenantId();
            }
        }
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => await Database.BeginTransactionAsync(cancellationToken);
}
