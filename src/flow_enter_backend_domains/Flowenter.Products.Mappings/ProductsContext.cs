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
    }
}
