using Flowenter.Products.IServices;
using Flowenter.Products.Mappings;
using Flowenter.Products.Models;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Products.Services;

public class ProductsServices : IProductsServices
{
    private readonly IDbContextFactory<ProductsContext> _factory;

    public ProductsServices(IDbContextFactory<ProductsContext> factory)
    {
        _factory = factory;
    }

    public async Task<IReadOnlyList<EnterpriseServiceDto>> GetServicesAsync(Guid enterpriseId, CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        return await context.Products
            .OfType<Service>()
            .Where(item => item.ProviderPartyId == enterpriseId)
            .OrderBy(item => item.Name)
            .Select(item => new EnterpriseServiceDto
            {
                ServiceId = item.Id!.Value,
                EnterpriseId = enterpriseId,
                Name = item.Name!,
                Description = item.Description,
                CreatedAtUtc = item.CreatedAtUtc,
                UpdatedAtUtc = item.UpdatedAtUtc,
                Revision = item.Revision
            })
            .ToListAsync(cancellationToken);
    }

    public async Task CreateServiceAsync(Guid enterpriseId, CreateEnterpriseServiceDto payload, CancellationToken cancellationToken = default)
    {
        var name = NormalizeName(payload.Name);

        using var context = _factory.CreateDbContext();

        var data = new Service
        {
            Id = Guid.NewGuid(),
            ProviderPartyId = enterpriseId,
            Name = name,
            Description = NormalizeDescription(payload.Description)
        };

        await context.AddAsync(data, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateServiceAsync(
        Guid enterpriseId,
        Guid serviceId,
        UpdateEnterpriseServiceDto payload,
        CancellationToken cancellationToken = default)
    {
        var name = NormalizeName(payload.Name);

        using var context = _factory.CreateDbContext();

        var service = await context.Products
            .OfType<Service>()
            .FirstOrDefaultAsync(item => item.Id == serviceId && item.ProviderPartyId == enterpriseId, cancellationToken);
        if (service == null)
        {
            return false;
        }

        service.Name = name;
        service.Description = NormalizeDescription(payload.Description);

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteServiceAsync(Guid enterpriseId, Guid serviceId, CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        var service = await context.Products
            .OfType<Service>()
            .FirstOrDefaultAsync(item => item.Id == serviceId && item.ProviderPartyId == enterpriseId, cancellationToken);
        if (service == null)
        {
            return false;
        }

        context.Remove(service);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string NormalizeName(string? name)
    {
        var normalized = name?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Service name is required.");
        }

        return normalized;
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}
