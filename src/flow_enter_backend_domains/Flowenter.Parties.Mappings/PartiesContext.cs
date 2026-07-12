using Flowenter.Domain.Models;
using Flowenter.Parties.Mappings.Extensions;
using Flowenter.Parties.Models.ContactMechanismModels;
using Flowenter.Parties.Models.FacilityModels;
using Flowenter.Parties.Models.GeographicBoundaryModels;
using Flowenter.Parties.Models.PartyModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Flowenter.Parties.Mappings;

public sealed class PartiesContext : DbContext
{
    // World
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<GenderType> GenderTypes => Set<GenderType>();

    // PartyModels
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

    // ContactMechanismModels
    public DbSet<ContactMechanismType> ContactMechanismTypes => Set<ContactMechanismType>();
    public DbSet<ContactMechanism> ContactMechanisms => Set<ContactMechanism>();

    // FinancialAccountModels
    public DbSet<FinancialAccount> FinancialAccounts => Set<FinancialAccount>();

    public readonly ITenantProvider _tenantProvider;

    // Party Relationships
    public DbSet<PartyRelationshipType> PartyRelationshipTypes => Set<PartyRelationshipType>();
    public DbSet<PartyRelationship> PartyRelationships => Set<PartyRelationship>();

    // Facilities
    public DbSet<FacilityType> FacilityTypes => Set<FacilityType>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<FacilityRoleType> FacilityRoleTypes => Set<FacilityRoleType>();
    public DbSet<FacilityRole> FacilityRoles => Set<FacilityRole>();

    public PartiesContext(DbContextOptions<PartiesContext> options,
        ITenantProvider tenantProvider) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("parties");

        //modelBuilder.Entity<Party>(builder =>
        //{
        //    builder.HasQueryFilter(pr => pr.TenantId == _tenantProvider.GetPartiesTenantId());
        //});
        modelBuilder.Entity<Person>();
        modelBuilder.Entity<Organization>();

        //modelBuilder.Entity<PartyRole>(builder =>
        //{
        //    builder.HasQueryFilter(pr => pr.TenantId == _tenantProvider.GetPartiesTenantId());
        //});
        modelBuilder.Entity<Enterprise>().ToTable("Enterprises");
        modelBuilder.Entity<Customer>().ToTable("Customers");
        modelBuilder.Entity<Branch>().ToTable("Branchs");

        //modelBuilder.Entity<ContactMechanism>(builder =>
        //{
        //    builder.HasQueryFilter(cm => cm.TenantId == _tenantProvider.GetPartiesTenantId());
        //});
        modelBuilder.Entity<ElectronicAddress>();
        modelBuilder.Entity<PostalAddress>();
        modelBuilder.Entity<TelecommunicationNumber>();

        modelBuilder.Entity<GeographicBoundary>();
        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Province>().ToTable("Provinces");
        modelBuilder.Entity<District>().ToTable("Districts");
        modelBuilder.Entity<Subdistrict>().ToTable("Sudistricts");

        modelBuilder.Entity<Province>(builder =>
        {
            builder.HasOne(e => e.Country)
                .WithMany()
                .HasForeignKey(e => e.CountryId)
                .OnDelete(DeleteBehavior.NoAction);
        });
        modelBuilder.Entity<District>(builder =>
        {
            builder.HasOne(e => e.Province)
                .WithMany()
                .HasForeignKey(e => e.ProvinceId)
                .OnDelete(DeleteBehavior.NoAction);
        });
        modelBuilder.Entity<Subdistrict>(builder =>
        {
            builder.HasOne(e => e.District)
                .WithMany()
                .HasForeignKey(e => e.DistrictId)
                .OnDelete(DeleteBehavior.NoAction);
        });

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

        modelBuilder.Entity<Employment>().ToTable("Employments");
        modelBuilder.Entity<Employment>(builder =>
        {
            builder.HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Employer)
                .WithMany()
                .HasForeignKey(e => e.EmployerId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<EnterpriseBranch>().ToTable("EnterpriseBranchs");
        modelBuilder.Entity<EnterpriseBranch>(builder =>
        {
            builder.HasOne(e => e.Enterprise)
                .WithMany()
                .HasForeignKey(e => e.EnterpriseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Branch)
                .WithMany()
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Facilities
        modelBuilder.Entity<Building>().ToTable("Buildings");
        modelBuilder.Entity<Floor>().ToTable("Floors");
        modelBuilder.Entity<Room>().ToTable("Rooms");
        modelBuilder.Entity<Bed>().ToTable("Beds");

        seeds(modelBuilder);
    }

    private void seeds(ModelBuilder modelBuilder) {

        modelBuilder.Entity<Language>().HasData([
            new Language
            {
                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                Code = Language.TH,
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new Language
            {
                Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                Code = Language.EN,
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            }
        ]);

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


        // Person Role Types
        modelBuilder.Entity<PartyRoleType>().HasData([
            // Organization Role Types
            new PartyRoleType
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Code = PartyRoleType.Enterprise,
                Name = "กิจการ/นิติบุคคล",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRoleType
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Code = PartyRoleType.Customer,
                Name = "ลูกค้า",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRoleType
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccd"),
                Code = PartyRoleType.Branch,
                Name = "สาขา",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },

            // Person Role Types
            new PartyRoleType
            {
                Id = Guid.Parse("aaaaaaaa-1111-1111-1111-111111111111"),
                Code = PartyRoleType.Administrator,
                Name = "ผู้บริหารหรือผู้จัดการสถานดูแล",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRoleType
            {
                Id = Guid.Parse("aaaaaaaa-2222-2222-2222-222222222222"),
                Code = PartyRoleType.CareManager,
                Name = "ผู้จัดการดูแลผู้ป่วยหรือผู้จัดการเคส",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRoleType
            {
                Id = Guid.Parse("aaaaaaaa-3333-3333-3333-333333333333"),
                Code = PartyRoleType.Nurse,
                Name = "พยาบาล",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRoleType
            {
                Id = Guid.Parse("aaaaaaaa-4444-4444-4444-444444444444"),
                Code = PartyRoleType.Caregiver,
                Name = "ผู้ดูแล",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRoleType
            {
                Id = Guid.Parse("aaaaaaaa-5555-5555-5555-555555555555"),
                Code = PartyRoleType.Physician,
                Name = "แพทย์",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRoleType
            {
                Id = Guid.Parse("aaaaaaaa-6666-6666-6666-666666666666"),
                Code = PartyRoleType.Pharmacist,
                Name = "เภสัชกร",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRoleType
            {
                Id = Guid.Parse("aaaaaaaa-7777-7777-7777-777777777777"),
                Code = PartyRoleType.Dietitian,
                Name = "นักกำหนดอาหารหรือนักโภชนาการ",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRoleType
            {
                Id = Guid.Parse("aaaaaaaa-8888-8888-8888-888888888888"),
                Code = PartyRoleType.KitchenStaff,
                Name = "พนักงานครัว",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRoleType
            {
                Id = Guid.Parse("aaaaaaaa-9999-9999-9999-999999999999"),
                Code = PartyRoleType.HousekeepingStaff,
                Name = "พนักงานทำความสะอาด",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRoleType
            {
                Id = Guid.Parse("bbbbbbbb-1111-1111-1111-111111111111"),
                Code = PartyRoleType.MaintenanceStaff,
                Name = "ช่างซ่อมบำรุง",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRoleType
            {
                Id = Guid.Parse("bbbbbbbb-2222-2222-2222-222222222222"),
                Code = PartyRoleType.LaundryStaff,
                Name = "พนักงานซักรีด",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRoleType
            {
                Id = Guid.Parse("bbbbbbbb-3333-3333-3333-333333333333"),
                Code = PartyRoleType.Receptionist,
                Name = "พนักงานต้อนรับ",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRoleType
            {
                Id = Guid.Parse("bbbbbbbb-4444-4444-4444-444444444444"),
                Code = PartyRoleType.Patient,
                Name = "ผู้ป่วย",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRoleType
            {
                Id = Guid.Parse("bbbbbbbb-5555-5555-5555-555555555555"),
                Code = PartyRoleType.SecurityGuard,
                Name = "เจ้าหน้าที่รักษาความปลอดภัย",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            }
        ]);

        modelBuilder.Entity<PartyRelationshipType>().HasData([
            new PartyRelationshipType
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                Code = PartyRelationshipType.Employment,
                Name = "การจ้างงาน",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new PartyRelationshipType
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffffe"),
                Code = PartyRelationshipType.EnterpriseBranch,
                Name = "สาขา",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            }
        ]);

        modelBuilder.Entity<FacilityType>().HasData([
            new FacilityType
            {
                Id = Guid.Parse("12121212-1212-1212-1212-121212121211"),
                Code = FacilityType.Building,
                Name = "อาคาร",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new FacilityType
            {
                Id = Guid.Parse("12121212-1212-1212-1212-121212121212"),
                Code = FacilityType.Floor,
                Name = "ชั้น",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new FacilityType
            {
                Id = Guid.Parse("12121212-1212-1212-1212-121212121213"),
                Code = FacilityType.Room,
                Name = "ห้อง",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new FacilityType
            {
                Id = Guid.Parse("12121212-1212-1212-1212-121212121214"),
                Code = FacilityType.Bed,
                Name = "เตียง",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            }
        ]);

        modelBuilder.Entity<FacilityRoleType>().HasData([
            new FacilityRoleType
            {
                Id = Guid.Parse("51515151-5151-5151-5151-515151515151"),
                Code = FacilityRoleType.Own,
                Name = "Own",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new FacilityRoleType
            {
                Id = Guid.Parse("62626262-6262-6262-6262-626262626262"),
                Code = FacilityRoleType.Rent,
                Name = "Rent",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new FacilityRoleType
            {
                Id = Guid.Parse("73737373-7373-7373-7373-737373737373"),
                Code = FacilityRoleType.Lease,
                Name = "Lease",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new FacilityRoleType
            {
                Id = Guid.Parse("84848484-8484-8484-8484-848484848484"),
                Code = FacilityRoleType.Use,
                Name = "Use",
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
            },
            new GeographicBoundaryType
            {
                Id = Guid.Parse("efefefef-efef-efef-efef-efefefefefef"),
                Code = GeographicBoundaryType.Province,
                Name = "Province",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new GeographicBoundaryType
            {
                Id = Guid.Parse("abababab-abab-abab-abab-abababababab"),
                Code = GeographicBoundaryType.District,
                Name = "District",
                CreatedBy = "seed",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Revision = 0
            },
            new GeographicBoundaryType
            {
                Id = Guid.Parse("cdcdcdcd-cdcd-cdcd-cdcd-cdcdcdcdcdcd"),
                Code = GeographicBoundaryType.Subdistrict,
                Name = "Subdistrict",
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
