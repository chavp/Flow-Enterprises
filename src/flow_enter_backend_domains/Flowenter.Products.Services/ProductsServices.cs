using Flowenter.Products.IServices;
using Flowenter.Products.Mappings;
using Flowenter.Products.Models;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Products.Services;

public class ProductsServices : IProductsServices
{
    private readonly IDbContextFactory<ProductsContext> _factory;
    private static readonly Dictionary<string, Type> ProductFeatureTypes = typeof(ProductFeature)
        .Assembly
        .GetTypes()
        .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(ProductFeature)))
        .ToDictionary(type => type.Name, type => type, StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, Type> ProductFeatureApplicabilityTypes = typeof(ProductFeatureApplicability)
        .Assembly
        .GetTypes()
        .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(ProductFeatureApplicability)))
        .ToDictionary(type => type.Name, type => type, StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, Type> PriceCoponentTypes = typeof(PriceCoponent)
        .Assembly
        .GetTypes()
        .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(PriceCoponent)))
        .ToDictionary(type => type.Name, type => type, StringComparer.OrdinalIgnoreCase);

    public ProductsServices(IDbContextFactory<ProductsContext> factory)
    {
        _factory = factory;
    }

    public Task<IReadOnlyList<string>> GetFeatureTypesAsync()
    {
        var types = ProductFeatureTypes
            .Keys
            .OrderBy(name => name)
            .ToList()
            .AsReadOnly();
        return Task.FromResult<IReadOnlyList<string>>(types);
    }

    public Task<IReadOnlyList<string>> GetFeatureApplicabilityTypesAsync()
    {
        var types = ProductFeatureApplicabilityTypes
            .Keys
            .OrderBy(name => name)
            .ToList()
            .AsReadOnly();
        return Task.FromResult<IReadOnlyList<string>>(types);
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
                ReleaseDate = item.ReleaseDate,
                DiscontinuedDate = item.DiscontinuedDate,
                SupportDiscontinuedDate = item.SupportDiscontinuedDate,
                HasCoverImage = item.CoverImage != null && item.CoverImage.Length > 0,
                CoverImageName = item.CoverImageName,
                FeatureCount = context.ProductFeatureApplicabilities.Count(app => app.ProductId == item.Id),
                CreatedAtUtc = item.CreatedAtUtc,
                UpdatedAtUtc = item.UpdatedAtUtc,
                Revision = item.Revision
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseGoodDto>> GetGoodsAsync(Guid enterpriseId, CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        return await context.Products
            .OfType<Good>()
            .Where(item => item.ProviderPartyId == enterpriseId)
            .OrderBy(item => item.Name)
            .Select(item => new EnterpriseGoodDto
            {
                GoodId = item.Id!.Value,
                EnterpriseId = enterpriseId,
                Name = item.Name!,
                Description = item.Description,
                ReleaseDate = item.ReleaseDate,
                DiscontinuedDate = item.DiscontinuedDate,
                SupportDiscontinuedDate = item.SupportDiscontinuedDate,
                HasCoverImage = item.CoverImage != null && item.CoverImage.Length > 0,
                CoverImageName = item.CoverImageName,
                FeatureCount = context.ProductFeatureApplicabilities.Count(app => app.ProductId == item.Id),
                CreatedAtUtc = item.CreatedAtUtc,
                UpdatedAtUtc = item.UpdatedAtUtc,
                Revision = item.Revision
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseServiceFeatureApplicabilityDto>> GetProductFeatureApplicabilitiesAsync(
        Guid enterpriseId,
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        var productExists = await context.Products
            .AnyAsync(item => item.Id == productId && item.ProviderPartyId == enterpriseId, cancellationToken);
        if (!productExists)
        {
            return [];
        }

        return await context.ProductFeatureApplicabilities
            .Where(item => item.ProductId == productId)
            .OrderBy(item => item.Order)
            .ThenBy(item => item.ProductFeature!.Code)
            .Select(item => new EnterpriseServiceFeatureApplicabilityDto
            {
                ProductFeatureApplicabilityId = item.Id!.Value,
                ProductFeatureId = item.ProductFeatureId!.Value,
                ProductFeatureCode = item.ProductFeature!.Code!,
                ProductFeatureTitle = item.ProductFeature.Title!,
                ProductFeatureApplicabilityType = item.ProductFeatureApplicabilityType!,
                Order = item.Order
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseServiceFeatureApplicabilityDto>> GetServiceFeatureApplicabilitiesAsync(
        Guid enterpriseId,
        Guid serviceId,
        CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        var serviceExists = await context.Products
            .OfType<Service>()
            .AnyAsync(item => item.Id == serviceId && item.ProviderPartyId == enterpriseId, cancellationToken);
        if (!serviceExists)
        {
            return [];
        }

        return await GetProductFeatureApplicabilitiesAsync(enterpriseId, serviceId, cancellationToken);
    }

    public async Task<IReadOnlyList<EnterpriseServicePriceCoponentDto>> GetServicePriceCoponentsAsync(
        Guid enterpriseId,
        Guid serviceId,
        CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        var serviceExists = await context.Products
            .OfType<Service>()
            .AnyAsync(item => item.Id == serviceId && item.ProviderPartyId == enterpriseId, cancellationToken);
        if (!serviceExists)
        {
            return [];
        }

        var data = await context.PriceCoponents
            .Where(item => item.SpecifiedForPartyId == serviceId)
            .OrderBy(item => item.FromDate)
            .ThenBy(item => item.Id)
            .Include(item => item.UnitOfMeasure)
            .ToListAsync(cancellationToken);

        var timeFrequencyMeasureIds = data
            .OfType<RecurringCharge>()
            .Where(item => item.TimeFrequencyMeasureId.HasValue)
            .Select(item => item.TimeFrequencyMeasureId!.Value)
            .ToHashSet();
        var timeFrequencyMeasureLookup = await context.UnitOfMeasures
            .OfType<TimeFrequencyMeasure>()
            .Where(item => timeFrequencyMeasureIds.Contains(item.Id!.Value))
            .ToDictionaryAsync(item => item.Id!.Value, item => item.Abbreviation!, cancellationToken);

        return data.Select(item =>
        {
            var recurringCharge = item as RecurringCharge;
            string? timeFrequencyMeasureAbbreviation = null;
            if (recurringCharge?.TimeFrequencyMeasureId is Guid timeFrequencyMeasureId &&
                timeFrequencyMeasureLookup.TryGetValue(timeFrequencyMeasureId, out var abbreviation))
            {
                timeFrequencyMeasureAbbreviation = abbreviation;
            }

            return new EnterpriseServicePriceCoponentDto
            {
                PriceCoponentId = item.Id!.Value,
                PriceCoponentType = item.PriceCoponentType!,
                Price = item.Price,
                Percent = item.Percent,
                UnitOfMeasureId = item.UnitOfMeasureId,
                UnitOfMeasureAbbreviation = item.UnitOfMeasure?.Abbreviation,
                TimeFrequencyMeasureId = recurringCharge?.TimeFrequencyMeasureId,
                TimeFrequencyMeasureAbbreviation = timeFrequencyMeasureAbbreviation,
                FromDate = item.FromDate,
                ThruDate = item.ThruDate,
                Description = item.Description
            };
        }).ToList();
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
                Name = item.Name!,
                IsGlobal = item.ProviderPartyId == null
            })
            .ToListAsync(cancellationToken);
    }

    public async Task CreateFeatureCategoryAsync(
        Guid enterpriseId,
        CreateProductFeatureCategoryDto payload,
        CancellationToken cancellationToken = default)
    {
        var name = NormalizeFeatureCategoryName(payload.Name);

        using var context = _factory.CreateDbContext();

        var data = new ProductFeatureCategory
        {
            Id = Guid.NewGuid(),
            ProviderPartyId = enterpriseId,
            Name = name
        };

        await context.AddAsync(data, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateFeatureCategoryAsync(
        Guid enterpriseId,
        Guid categoryId,
        UpdateProductFeatureCategoryDto payload,
        CancellationToken cancellationToken = default)
    {
        var name = NormalizeFeatureCategoryName(payload.Name);

        using var context = _factory.CreateDbContext();

        var category = await context.ProductFeatureCategories
            .FirstOrDefaultAsync(item => item.Id == categoryId && item.ProviderPartyId == enterpriseId, cancellationToken);
        if (category == null)
        {
            return false;
        }

        category.Name = name;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteFeatureCategoryAsync(
        Guid enterpriseId,
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        var category = await context.ProductFeatureCategories
            .FirstOrDefaultAsync(item => item.Id == categoryId && item.ProviderPartyId == enterpriseId, cancellationToken);
        if (category == null)
        {
            return false;
        }

        var inUse = await context.ProductFeatures.AnyAsync(
            item => item.ProductFeatureCategoryId == categoryId && item.ProviderPartyId == enterpriseId,
            cancellationToken);
        if (inUse)
        {
            throw new ArgumentException("Product feature category is being used by product features.");
        }

        context.Remove(category);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<EnterpriseProductFeatureDto>> GetFeaturesAsync(
        Guid enterpriseId,
        CancellationToken cancellationToken = default)
    {
        using var context = _factory.CreateDbContext();

        return await context.ProductFeatures
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
            Description = NormalizeDescription(payload.Description),
            ReleaseDate = payload.ReleaseDate,
            DiscontinuedDate = payload.DiscontinuedDate,
            SupportDiscontinuedDate = payload.SupportDiscontinuedDate
        };
        if (payload.CoverImage.Length > 0)
        {
            data.CoverImage = payload.CoverImage;
            data.CoverImageName = NormalizeCoverImageName(payload.CoverImageName);
        }

        await context.AddAsync(data, cancellationToken);
        await SyncServiceFeatureApplicabilitiesAsync(
            context,
            enterpriseId,
            data.Id!.Value,
            payload.ProductFeatureApplicabilities,
            cancellationToken);
        await SyncServicePriceCoponentsAsync(
            context,
            enterpriseId,
            data.Id!.Value,
            payload.PriceCoponents,
            cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateFeatureAsync(
        Guid enterpriseId,
        CreateEnterpriseProductFeatureDto payload,
        CancellationToken cancellationToken = default)
    {
        var productFeatureType = NormalizeProductFeatureType(payload.ProductFeatureType);
        var code = NormalizeCode(payload.Code);
        var title = NormalizeTitle(payload.Title);

        using var context = _factory.CreateDbContext();

        await EnsureFeatureCategoryExistsAsync(context, enterpriseId, payload.ProductFeatureCategoryId, cancellationToken);

        var data = CreateProductFeature(productFeatureType);
        data.Id = Guid.NewGuid();
        data.ProviderPartyId = enterpriseId;
        data.ProductFeatureType = productFeatureType;
        data.ProductFeatureCategoryId = payload.ProductFeatureCategoryId;
        data.Code = code;
        data.Title = title;
        data.Description = NormalizeDescription(payload.Description);

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
        service.ReleaseDate = payload.ReleaseDate;
        service.DiscontinuedDate = payload.DiscontinuedDate;
        service.SupportDiscontinuedDate = payload.SupportDiscontinuedDate;
        if (payload.RemoveCoverImage)
        {
            service.CoverImage = [];
            service.CoverImageName = null;
        }
        else if (payload.CoverImage.Length > 0)
        {
            service.CoverImage = payload.CoverImage;
            service.CoverImageName = NormalizeCoverImageName(payload.CoverImageName);
        }
        await SyncServiceFeatureApplicabilitiesAsync(
            context,
            enterpriseId,
            serviceId,
            payload.ProductFeatureApplicabilities,
            cancellationToken);
        await SyncServicePriceCoponentsAsync(
            context,
            enterpriseId,
            serviceId,
            payload.PriceCoponents,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateFeatureAsync(
        Guid enterpriseId,
        Guid featureId,
        UpdateEnterpriseProductFeatureDto payload,
        CancellationToken cancellationToken = default)
    {
        var productFeatureType = NormalizeProductFeatureType(payload.ProductFeatureType);
        var code = NormalizeCode(payload.Code);
        var title = NormalizeTitle(payload.Title);

        using var context = _factory.CreateDbContext();

        var feature = await context.ProductFeatures
            .FirstOrDefaultAsync(item => item.Id == featureId && item.ProviderPartyId == enterpriseId, cancellationToken);
        if (feature == null)
        {
            return false;
        }

        await EnsureFeatureCategoryExistsAsync(context, enterpriseId, payload.ProductFeatureCategoryId, cancellationToken);

        feature.ProductFeatureCategoryId = payload.ProductFeatureCategoryId;
        feature.ProductFeatureType = productFeatureType;
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

    private static string NormalizeProductFeatureType(string? productFeatureType)
    {
        var normalized = productFeatureType?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Product feature type is required.");
        }

        if (!ProductFeatureTypes.TryGetValue(normalized, out var resolvedType))
        {
            throw new ArgumentException("Product feature type is invalid.");
        }

        return resolvedType.Name;
    }

    private static string NormalizeFeatureCategoryName(string? name)
    {
        var normalized = name?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Product feature category name is required.");
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

    private static string? NormalizeCoverImageName(string? coverImageName)
    {
        if (string.IsNullOrWhiteSpace(coverImageName))
        {
            return null;
        }

        return coverImageName.Trim();
    }

    private static string NormalizeProductFeatureApplicabilityType(string? productFeatureApplicabilityType)
    {
        var normalized = productFeatureApplicabilityType?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Product feature applicability type is required.");
        }

        if (!ProductFeatureApplicabilityTypes.TryGetValue(normalized, out var resolvedType))
        {
            throw new ArgumentException("Product feature applicability type is invalid.");
        }

        return resolvedType.Name;
    }

    private static string NormalizePriceCoponentType(string? priceCoponentType)
    {
        var normalized = priceCoponentType?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Price coponent type is required.");
        }

        if (!PriceCoponentTypes.TryGetValue(normalized, out var resolvedType))
        {
            throw new ArgumentException("Price coponent type is invalid.");
        }

        return resolvedType.Name;
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

    private static ProductFeature CreateProductFeature(string productFeatureType)
    {
        if (!ProductFeatureTypes.TryGetValue(productFeatureType, out var resolvedType))
        {
            throw new ArgumentException("Product feature type is invalid.");
        }

        var instance = Activator.CreateInstance(resolvedType) as ProductFeature;
        if (instance == null)
        {
            throw new ArgumentException("Product feature type is invalid.");
        }

        return instance;
    }

    private static ProductFeatureApplicability CreateProductFeatureApplicability(string productFeatureApplicabilityType)
    {
        if (!ProductFeatureApplicabilityTypes.TryGetValue(productFeatureApplicabilityType, out var resolvedType))
        {
            throw new ArgumentException("Product feature applicability type is invalid.");
        }

        var instance = Activator.CreateInstance(resolvedType) as ProductFeatureApplicability;
        if (instance == null)
        {
            throw new ArgumentException("Product feature applicability type is invalid.");
        }

        return instance;
    }

    private static PriceCoponent CreatePriceCoponent(string priceCoponentType)
    {
        if (!PriceCoponentTypes.TryGetValue(priceCoponentType, out var resolvedType))
        {
            throw new ArgumentException("Price coponent type is invalid.");
        }

        var instance = Activator.CreateInstance(resolvedType) as PriceCoponent;
        if (instance == null)
        {
            throw new ArgumentException("Price coponent type is invalid.");
        }

        return instance;
    }

    private static async Task SyncServiceFeatureApplicabilitiesAsync(
        ProductsContext context,
        Guid enterpriseId,
        Guid serviceId,
        IReadOnlyCollection<UpsertEnterpriseServiceFeatureApplicabilityDto>? productFeatureApplicabilities,
        CancellationToken cancellationToken)
    {
        var applicabilities = productFeatureApplicabilities ?? [];
        var featureIds = applicabilities.Select(item => item.ProductFeatureId).ToHashSet();
        if (featureIds.Count != applicabilities.Count)
        {
            throw new ArgumentException("Product feature applicability contains duplicate product feature.");
        }

        var enterpriseFeatureIds = await context.ProductFeatures
            .Where(item => item.ProviderPartyId == enterpriseId && featureIds.Contains(item.Id!.Value))
            .Select(item => item.Id!.Value)
            .ToListAsync(cancellationToken);
        if (enterpriseFeatureIds.Count != featureIds.Count)
        {
            throw new ArgumentException("One or more product features were not found for this enterprise.");
        }

        var existing = await context.ProductFeatureApplicabilities
            .Where(item => item.ProductId == serviceId)
            .ToListAsync(cancellationToken);
        if (existing.Count > 0)
        {
            context.RemoveRange(existing);
        }

        foreach (var applicability in applicabilities)
        {
            var applicabilityType = NormalizeProductFeatureApplicabilityType(applicability.ProductFeatureApplicabilityType);
            var data = CreateProductFeatureApplicability(applicabilityType);
            data.Id = Guid.NewGuid();
            data.ProductId = serviceId;
            data.ProductFeatureId = applicability.ProductFeatureId;
            data.ProductFeatureApplicabilityType = applicabilityType;
            data.Order = applicability.Order;
            await context.AddAsync(data, cancellationToken);
        }
    }

    private static async Task SyncServicePriceCoponentsAsync(
        ProductsContext context,
        Guid enterpriseId,
        Guid serviceId,
        IReadOnlyCollection<UpsertEnterpriseServicePriceCoponentDto>? priceCoponents,
        CancellationToken cancellationToken)
    {
        _ = enterpriseId;
        var components = priceCoponents ?? [];

        var unitOfMeasureIds = components
            .Where(item => item.UnitOfMeasureId.HasValue)
            .Select(item => item.UnitOfMeasureId!.Value)
            .ToHashSet();
        var existingUnitOfMeasureIds = await context.UnitOfMeasures
            .Where(item => unitOfMeasureIds.Contains(item.Id!.Value))
            .Select(item => item.Id!.Value)
            .ToListAsync(cancellationToken);
        if (existingUnitOfMeasureIds.Count != unitOfMeasureIds.Count)
        {
            throw new ArgumentException("One or more unit of measures were not found.");
        }

        var recurringTimeFrequencyMeasureIds = components
            .Where(item => string.Equals(item.PriceCoponentType, nameof(RecurringCharge), StringComparison.OrdinalIgnoreCase))
            .Select(item => item.TimeFrequencyMeasureId)
            .ToList();
        if (recurringTimeFrequencyMeasureIds.Any(item => item == null))
        {
            throw new ArgumentException("RecurringCharge requires TimeFrequencyMeasure.");
        }

        var resolvedRecurringIds = recurringTimeFrequencyMeasureIds
            .Where(item => item.HasValue)
            .Select(item => item!.Value)
            .ToHashSet();
        if (resolvedRecurringIds.Count > 0)
        {
            var existingTimeFrequencyMeasureIds = await context.UnitOfMeasures
                .OfType<TimeFrequencyMeasure>()
                .Where(item => resolvedRecurringIds.Contains(item.Id!.Value))
                .Select(item => item.Id!.Value)
                .ToListAsync(cancellationToken);
            if (existingTimeFrequencyMeasureIds.Count != resolvedRecurringIds.Count)
            {
                throw new ArgumentException("One or more TimeFrequencyMeasure were not found.");
            }
        }

        var existing = await context.PriceCoponents
            .Where(item => item.SpecifiedForPartyId == serviceId)
            .ToListAsync(cancellationToken);
        if (existing.Count > 0)
        {
            context.RemoveRange(existing);
        }

        foreach (var item in components)
        {
            var priceCoponentType = NormalizePriceCoponentType(item.PriceCoponentType);
            var data = CreatePriceCoponent(priceCoponentType);
            data.Id = Guid.NewGuid();
            data.PriceCoponentType = priceCoponentType;
            data.SpecifiedForPartyId = serviceId;
            data.UnitOfMeasureId = item.UnitOfMeasureId;
            data.Price = item.Price;
            data.Percent = item.Percent;
            data.FromDate = item.FromDate ?? DateOnly.FromDateTime(DateTime.Today);
            data.ThruDate = item.ThruDate ?? DateOnly.MaxValue;
            data.Description = NormalizeDescription(item.Description);

            if (data is RecurringCharge recurringCharge)
            {
                recurringCharge.TimeFrequencyMeasureId = item.TimeFrequencyMeasureId!.Value;
            }

            await context.AddAsync(data, cancellationToken);
        }
    }
}
