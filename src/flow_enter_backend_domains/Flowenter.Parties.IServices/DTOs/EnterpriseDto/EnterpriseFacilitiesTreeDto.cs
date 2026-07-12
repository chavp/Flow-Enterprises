namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record EnterpriseFacilitiesTreeDto
{
    public List<BuildingFacilitiesNodeDto> Buildings { get; init; } = [];
}

public record BuildingFacilitiesNodeDto
{
    public BuildingDto Building { get; init; } = new();
    public List<FloorFacilitiesNodeDto> Floors { get; init; } = [];
}

public record FloorFacilitiesNodeDto
{
    public FloorDto Floor { get; init; } = new();
    public List<RoomFacilitiesNodeDto> Rooms { get; init; } = [];
}

public record RoomFacilitiesNodeDto
{
    public RoomDto Room { get; init; } = new();
    public List<BedDto> Beds { get; init; } = [];
}
