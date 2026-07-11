using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record UpdateRoomDto
{
    [Required, StringLength(100)]
    public string Number { get; init; } = string.Empty;
}
