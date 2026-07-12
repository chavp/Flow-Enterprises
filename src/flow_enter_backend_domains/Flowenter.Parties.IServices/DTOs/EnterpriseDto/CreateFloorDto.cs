using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record CreateFloorDto
{
    [Required]
    public Guid BuildingId { get; init; }

    [Required]
    public int Level { get; init; }

    [StringLength(500)]
    public string? Description { get; init; }
}
