using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record UpdateBedDto
{
    [Required, StringLength(100)]
    public string Number { get; init; } = string.Empty;

    [Required]
    public Guid RoomId { get; init; }
}
