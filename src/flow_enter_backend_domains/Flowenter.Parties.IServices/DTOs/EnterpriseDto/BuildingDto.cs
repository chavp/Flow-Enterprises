namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record BuildingDto
{
    public Guid BuildingId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public List<Guid> BranchIds { get; init; } = [];
    public List<string> BranchLegalNames { get; init; } = [];
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public ulong Revision { get; init; }
}
