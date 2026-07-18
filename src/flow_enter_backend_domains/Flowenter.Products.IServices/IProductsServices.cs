namespace Flowenter.Products.IServices;

public interface IProductsServices
{
    Task<IReadOnlyList<ProductFeatureCategoryDto>> GetFeatureCategoriesAsync(Guid enterpriseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EnterpriseProductFeatureDto>> GetFeaturesAsync(Guid enterpriseId, CancellationToken cancellationToken = default);
    Task CreateFeatureAsync(Guid enterpriseId, CreateEnterpriseProductFeatureDto payload, CancellationToken cancellationToken = default);
    Task<bool> UpdateFeatureAsync(Guid enterpriseId, Guid featureId, UpdateEnterpriseProductFeatureDto payload, CancellationToken cancellationToken = default);
    Task<bool> DeleteFeatureAsync(Guid enterpriseId, Guid featureId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EnterpriseServiceDto>> GetServicesAsync(Guid enterpriseId, CancellationToken cancellationToken = default);
    Task CreateServiceAsync(Guid enterpriseId, CreateEnterpriseServiceDto payload, CancellationToken cancellationToken = default);
    Task<bool> UpdateServiceAsync(Guid enterpriseId, Guid serviceId, UpdateEnterpriseServiceDto payload, CancellationToken cancellationToken = default);
    Task<bool> DeleteServiceAsync(Guid enterpriseId, Guid serviceId, CancellationToken cancellationToken = default);
}

public sealed class ProductFeatureCategoryDto
{
    public Guid ProductFeatureCategoryId { get; set; }
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
    public Guid ProductFeatureCategoryId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed class UpdateEnterpriseProductFeatureDto
{
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
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public ulong Revision { get; set; }
}

public sealed class CreateEnterpriseServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed class UpdateEnterpriseServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
