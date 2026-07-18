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

    public async Task<IReadOnlyList<ProductFeatureCategoryDto>> GetFeatureCategoriesAsync(
        Guid enterpriseId,
        CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        return await context.ProductFeatureCategories
            .Where(item => item.ProviderPartyId == null || item.ProviderPartyId == enterpriseId)
            .OrderBy(item => item.Name)
            .Select(item => new ProductFeatureCategoryDto
            {
                ProductFeatureCategoryId = item.Id!.Value,
                Name = item.Name!
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseProductFeatureDto>> GetFeaturesAsync(
        Guid enterpriseId,
        CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        return await context.ProductFeatures
            .OfType<ServiceFeature>()
            .Where(item => item.ProviderPartyId == enterpriseId)
            .OrderBy(item => item.Code)
            .ThenBy(item => item.Title)
            .Select(item => new EnterpriseProductFeatureDto
            {
                ProductFeatureId = item.Id!.Value,
                EnterpriseId = enterpriseId,
                ProductFeatureCategoryId = item.ProductFeatureCategoryId!.Value,
                ProductFeatureCategoryName = item.ProductFeatureCategory!.Name!,
                ProductFeatureType = item.ProductFeatureType!,
                Code = item.Code!,
                Title = item.Title!,
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

    public async Task CreateFeatureAsync(
        Guid enterpriseId,
        CreateEnterpriseProductFeatureDto payload,
        CancellationToken cancellationToken = default)
    {
        var code = NormalizeCode(payload.Code);
        var title = NormalizeTitle(payload.Title);

        using var context = _factory.CreateDbContext();

        await EnsureFeatureCategoryExistsAsync(context, enterpriseId, payload.ProductFeatureCategoryId, cancellationToken);

        var data = new ServiceFeature
        {
            Id = Guid.NewGuid(),
            ProviderPartyId = enterpriseId,
            ProductFeatureType = nameof(ServiceFeature),
            ProductFeatureCategoryId = payload.ProductFeatureCategoryId,
            Code = code,
            Title = title,
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

    public async Task<bool> UpdateFeatureAsync(
        Guid enterpriseId,
        Guid featureId,
        UpdateEnterpriseProductFeatureDto payload,
        CancellationToken cancellationToken = default)
    {
        var code = NormalizeCode(payload.Code);
        var title = NormalizeTitle(payload.Title);

        using var context = _factory.CreateDbContext();

        var feature = await context.ProductFeatures
            .OfType<ServiceFeature>()
            .FirstOrDefaultAsync(item => item.Id == featureId && item.ProviderPartyId == enterpriseId, cancellationToken);
        if (feature == null)
        {
            return false;
        }

        await EnsureFeatureCategoryExistsAsync(context, enterpriseId, payload.ProductFeatureCategoryId, cancellationToken);

        feature.ProductFeatureCategoryId = payload.ProductFeatureCategoryId;
        feature.Code = code;
        feature.Title = title;
        feature.Description = NormalizeDescription(payload.Description);

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

    public async Task<bool> DeleteFeatureAsync(Guid enterpriseId, Guid featureId, CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        var feature = await context.ProductFeatures
            .OfType<ServiceFeature>()
            .FirstOrDefaultAsync(item => item.Id == featureId && item.ProviderPartyId == enterpriseId, cancellationToken);
        if (feature == null)
        {
            return false;
        }

        context.Remove(feature);
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

    private static string NormalizeCode(string? code)
    {
        var normalized = code?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Product feature code is required.");
        }

        return normalized;
    }

    private static string NormalizeTitle(string? title)
    {
        var normalized = title?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Product feature title is required.");
        }

        return normalized;
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    private static async Task EnsureFeatureCategoryExistsAsync(
        ProductsContext context,
        Guid enterpriseId,
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var categoryExists = await context.ProductFeatureCategories.AnyAsync(
            item => item.Id == categoryId && (item.ProviderPartyId == null || item.ProviderPartyId == enterpriseId),
            cancellationToken);
        if (!categoryExists)
        {
            throw new ArgumentException("Product feature category not found.");
        }
    }
}
