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
    public DbSet<PartyType> PartyTypes => Set<PartyType>();
    public DbSet<PartyRoleType> PartyRoleTypes => Set<PartyRoleType>();
    public DbSet<LegalStructure> LegalStructures => Set<LegalStructure>();
    public DbSet<PartyCategory> PartyCategories => Set<PartyCategory>();

    public DbSet<Party> Parties => Set<Party>();
    public DbSet<PartyRole> PartyRoles => Set<PartyRole>();
    public DbSet<PartyContactMechanism> PartyContactMechanisms => Set<PartyContactMechanism>();
    public DbSet<PartyClassification> PartyClassifications => Set<PartyClassification>();

    // GeographicBoundaryModels
    public DbSet<GeographicBoundaryType> GeographicBoundaryTypes => Set<GeographicBoundaryType>();
    public DbSet<GeographicBoundary> GeographicBoundaries => Set<GeographicBoundary>();

    // ContactMechanismModels
    public DbSet<ContactMechanismType> ContactMechanismTypes => Set<ContactMechanismType>();
    public DbSet<ContactMechanism> ContactMechanisms => Set<ContactMechanism>();

    // FinancialAccountModels
    public DbSet<FinancialAccount> FinancialAccounts => Set<FinancialAccount>();

    public PartiesContext(DbContextOptions<PartiesContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("parties");

        modelBuilder.Entity<Party>();
        modelBuilder.Entity<Person>();
        modelBuilder.Entity<Organization>();

        modelBuilder.Entity<PartyRole>();
        modelBuilder.Entity<Enterprise>();
        modelBuilder.Entity<Customer>();

        modelBuilder.Entity<ContactMechanism>();
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
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => await Database.BeginTransactionAsync(cancellationToken);
}
