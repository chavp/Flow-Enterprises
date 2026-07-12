using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record UpdateRoomDto
{
    [Required, StringLength(100)]
    public string Number { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; init; }
}
