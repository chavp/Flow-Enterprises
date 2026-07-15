using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Products.Mappings;

public class ProductsDbContextFactory : IDbContextFactory<ProductsContext>
{
    private readonly ITenantProvider _tenantProvider;

    public ProductsDbContextFactory(ITenantProvider tenantProvider)
    {
        _tenantProvider = tenantProvider;
    }

    public ProductsContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ProductsContext>()
            .UseSqlServer(_tenantProvider.GetProductsTenantConnectionString())
            .Options;

        return new ProductsContext(options, _tenantProvider);
    }
}
