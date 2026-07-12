namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record BedDto
{
    public Guid BedId { get; init; }
    public string Number { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid RoomId { get; init; }
    public string RoomNumber { get; init; } = string.Empty;
    public Guid? FloorId { get; init; }
    public int? FloorLevel { get; init; }
    public Guid? BuildingId { get; init; }
    public string? BuildingName { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public ulong Revision { get; init; }
}
