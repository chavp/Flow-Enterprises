namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record FloorDto
{
    public Guid FloorId { get; init; }
    public int Level { get; init; }
    public string? Description { get; init; }
    public Guid BuildingId { get; init; }
    public string BuildingName { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public ulong Revision { get; init; }
}
