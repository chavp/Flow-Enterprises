namespace Flowenter.Products.IServices;

public interface IProductsServices
{
    Task<IReadOnlyList<EnterpriseServiceDto>> GetServicesAsync(Guid enterpriseId, CancellationToken cancellationToken = default);
    Task CreateServiceAsync(Guid enterpriseId, CreateEnterpriseServiceDto payload, CancellationToken cancellationToken = default);
    Task<bool> UpdateServiceAsync(Guid enterpriseId, Guid serviceId, UpdateEnterpriseServiceDto payload, CancellationToken cancellationToken = default);
    Task<bool> DeleteServiceAsync(Guid enterpriseId, Guid serviceId, CancellationToken cancellationToken = default);
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
