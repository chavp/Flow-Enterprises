using Flowenter.Domain.Models;
using Flowenter.Products.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Flowenter.Products.Mappings;

public sealed class ProductsContext : DbContext
{
    public readonly ITenantProvider _tenantProvider;

    public DbSet<Product> Products => Set<Product>();

    public DbSet<ProductFeatureCategory> ProductFeatureCategories => Set<ProductFeatureCategory>();
    public DbSet<ProductFeature> ProductFeatures => Set<ProductFeature>();
    public DbSet<ProductFeatureApplicability> ProductFeatureApplicabilities => Set<ProductFeatureApplicability>();

    public DbSet<PriceCoponent> PriceCoponents => Set<PriceCoponent>();
    public DbSet<UnitOfMeasure> UnitOfMeasures => Set<UnitOfMeasure>();

    public ProductsContext(DbContextOptions<ProductsContext> options,
        ITenantProvider tenantProvider) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("products");

        modelBuilder.Entity<Good>().ToTable("Goods");
        modelBuilder.Entity<Service>().ToTable("Services");

        modelBuilder.Entity<ProductFeatureApplicability>()
            .HasDiscriminator(b => b.ProductFeatureApplicabilityType);
        modelBuilder.Entity<RequiredFeature>();
        modelBuilder.Entity<StandardFeature>();
        modelBuilder.Entity<OptionalFeature>();
        modelBuilder.Entity<SelectableFeature>();

        modelBuilder.Entity<ProductFeature>()
            .HasDiscriminator(b => b.ProductFeatureType);
        modelBuilder.Entity<ServiceFeature>();

        modelBuilder.Entity<PriceCoponent>()
            .HasDiscriminator(b => b.PriceCoponentType);
        modelBuilder.Entity<BasePrice>();
        modelBuilder.Entity<RecurringCharge>(builder =>
        {
            builder.HasOne(e => e.TimeFrequencyMeasure)
                .WithMany()
                .HasForeignKey(e => e.TimeFrequencyMeasureId)
                .OnDelete(DeleteBehavior.NoAction);
        });
        modelBuilder.Entity<PriceCoponent>(builder =>
        {
            builder.HasOne(e => e.UnitOfMeasure)
                .WithMany()
                .HasForeignKey(e => e.UnitOfMeasureId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<UnitOfMeasure>()
            .HasDiscriminator(b => b.UnitOfMeasureType);
        modelBuilder.Entity<CurrencyMeasure>();
        modelBuilder.Entity<TimeFrequencyMeasure>();
    }
}
