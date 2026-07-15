using Flowenter.Domain.Models;
using Flowenter.Products.Models;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Products.Mappings;

public sealed class ProductsContext : DbContext
{
    public readonly ITenantProvider _tenantProvider;

    public DbSet<Product> Products => Set<Product>();

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
    }
}
