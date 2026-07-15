using Flowenter.Domain.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Products.Mappings;

public class ProductsDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ProductsContext>
{
    public ProductsContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProductsContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1434;TrustServerCertificate=True;Database=flow-enter-products;User Id=sa;Password=Admin@1234");
        return new ProductsContext(optionsBuilder.Options, new DevTenantProvider());
    }
}
