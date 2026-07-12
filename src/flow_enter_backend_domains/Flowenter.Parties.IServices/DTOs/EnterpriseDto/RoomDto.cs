namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record RoomDto
{
    public Guid RoomId { get; init; }
    public string Number { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int BedCount { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public ulong Revision { get; init; }
}
