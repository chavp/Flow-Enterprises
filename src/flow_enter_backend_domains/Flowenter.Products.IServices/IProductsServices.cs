namespace Flowenter.Products.IServices;

public interface IProductsServices
{
    Task<IReadOnlyList<string>> GetFeatureTypesAsync();
    Task<IReadOnlyList<string>> GetFeatureApplicabilityTypesAsync();
    Task<IReadOnlyList<ProductFeatureCategoryDto>> GetFeatureCategoriesAsync(Guid enterpriseId, CancellationToken cancellationToken = default);
    Task CreateFeatureCategoryAsync(Guid enterpriseId, CreateProductFeatureCategoryDto payload, CancellationToken cancellationToken = default);
    Task<bool> UpdateFeatureCategoryAsync(Guid enterpriseId, Guid categoryId, UpdateProductFeatureCategoryDto payload, CancellationToken cancellationToken = default);
    Task<bool> DeleteFeatureCategoryAsync(Guid enterpriseId, Guid categoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EnterpriseProductFeatureDto>> GetFeaturesAsync(Guid enterpriseId, CancellationToken cancellationToken = default);
    Task CreateFeatureAsync(Guid enterpriseId, CreateEnterpriseProductFeatureDto payload, CancellationToken cancellationToken = default);
    Task<bool> UpdateFeatureAsync(Guid enterpriseId, Guid featureId, UpdateEnterpriseProductFeatureDto payload, CancellationToken cancellationToken = default);
    Task<bool> DeleteFeatureAsync(Guid enterpriseId, Guid featureId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EnterpriseServiceDto>> GetServicesAsync(Guid enterpriseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EnterpriseGoodDto>> GetGoodsAsync(Guid enterpriseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EnterpriseServiceFeatureApplicabilityDto>> GetProductFeatureApplicabilitiesAsync(Guid enterpriseId, Guid productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EnterpriseServiceFeatureApplicabilityDto>> GetServiceFeatureApplicabilitiesAsync(Guid enterpriseId, Guid serviceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EnterpriseServicePriceCoponentDto>> GetServicePriceCoponentsAsync(Guid enterpriseId, Guid serviceId, CancellationToken cancellationToken = default);
    Task CreateServiceAsync(Guid enterpriseId, CreateEnterpriseServiceDto payload, CancellationToken cancellationToken = default);
    Task<bool> UpdateServiceAsync(Guid enterpriseId, Guid serviceId, UpdateEnterpriseServiceDto payload, CancellationToken cancellationToken = default);
    Task<bool> DeleteServiceAsync(Guid enterpriseId, Guid serviceId, CancellationToken cancellationToken = default);
}

public sealed class ProductFeatureCategoryDto
{
    public Guid ProductFeatureCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsGlobal { get; set; }
}

public sealed class CreateProductFeatureCategoryDto
{
    public string Name { get; set; } = string.Empty;
}

public sealed class UpdateProductFeatureCategoryDto
{
    public string Name { get; set; } = string.Empty;
}

public sealed class EnterpriseProductFeatureDto
{
    public Guid ProductFeatureId { get; set; }
    public Guid EnterpriseId { get; set; }
    public Guid ProductFeatureCategoryId { get; set; }
    public string ProductFeatureCategoryName { get; set; } = string.Empty;
    public string ProductFeatureType { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public ulong Revision { get; set; }
}

public sealed class CreateEnterpriseProductFeatureDto
{
    public string ProductFeatureType { get; set; } = string.Empty;
    public Guid ProductFeatureCategoryId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed class UpdateEnterpriseProductFeatureDto
{
    public string ProductFeatureType { get; set; } = string.Empty;
    public Guid ProductFeatureCategoryId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed class EnterpriseServiceDto
{
    public Guid ServiceId { get; set; }
    public Guid EnterpriseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? PriceDisplay { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public DateOnly? DiscontinuedDate { get; set; }
    public DateOnly? SupportDiscontinuedDate { get; set; }
    public bool HasCoverImage { get; set; }
    public string? CoverImageName { get; set; }
    public int FeatureCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public ulong Revision { get; set; }
}

public sealed class EnterpriseGoodDto
{
    public Guid GoodId { get; set; }
    public Guid EnterpriseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public DateOnly? DiscontinuedDate { get; set; }
    public DateOnly? SupportDiscontinuedDate { get; set; }
    public bool HasCoverImage { get; set; }
    public string? CoverImageName { get; set; }
    public int FeatureCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public ulong Revision { get; set; }
}

public sealed class CreateEnterpriseServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public DateOnly? DiscontinuedDate { get; set; }
    public DateOnly? SupportDiscontinuedDate { get; set; }
    public byte[] CoverImage { get; set; } = [];
    public string? CoverImageName { get; set; }
    public List<UpsertEnterpriseServiceFeatureApplicabilityDto> ProductFeatureApplicabilities { get; set; } = [];
    public List<UpsertEnterpriseServicePriceCoponentDto> PriceCoponents { get; set; } = [];
}

public sealed class UpdateEnterpriseServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public DateOnly? DiscontinuedDate { get; set; }
    public DateOnly? SupportDiscontinuedDate { get; set; }
    public byte[] CoverImage { get; set; } = [];
    public string? CoverImageName { get; set; }
    public bool RemoveCoverImage { get; set; }
    public List<UpsertEnterpriseServiceFeatureApplicabilityDto> ProductFeatureApplicabilities { get; set; } = [];
    public List<UpsertEnterpriseServicePriceCoponentDto> PriceCoponents { get; set; } = [];
}

public sealed class EnterpriseServiceFeatureApplicabilityDto
{
    public Guid ProductFeatureApplicabilityId { get; set; }
    public Guid ProductFeatureId { get; set; }
    public string ProductFeatureCode { get; set; } = string.Empty;
    public string ProductFeatureTitle { get; set; } = string.Empty;
    public string ProductFeatureApplicabilityType { get; set; } = string.Empty;
    public int Order { get; set; }
}

public sealed class UpsertEnterpriseServiceFeatureApplicabilityDto
{
    public Guid ProductFeatureId { get; set; }
    public string ProductFeatureApplicabilityType { get; set; } = string.Empty;
    public int Order { get; set; }
}

public sealed class EnterpriseServicePriceCoponentDto
{
    public Guid PriceCoponentId { get; set; }
    public string PriceCoponentType { get; set; } = string.Empty;
    public Guid? SpecifiedForPartyId { get; set; }
    public decimal? Price { get; set; }
    public decimal? Percent { get; set; }
    public Guid? UnitOfMeasureId { get; set; }
    public string? UnitOfMeasureAbbreviation { get; set; }
    public Guid? TimeFrequencyMeasureId { get; set; }
    public string? TimeFrequencyMeasureAbbreviation { get; set; }
    public DateOnly FromDate { get; set; }
    public DateOnly ThruDate { get; set; }
    public string? Description { get; set; }
}

public sealed class UpsertEnterpriseServicePriceCoponentDto
{
    public string PriceCoponentType { get; set; } = string.Empty;
    public Guid? SpecifiedForPartyId { get; set; }
    public decimal? Price { get; set; }
    public decimal? Percent { get; set; }
    public Guid? UnitOfMeasureId { get; set; }
    public Guid? TimeFrequencyMeasureId { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ThruDate { get; set; }
    public string? Description { get; set; }
}
