using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record UpdateEmploymentEffectiveDateDto
{
    [Required]
    public DateOnly FromDate { get; init; }

    [Required]
    public DateOnly ThruDate { get; init; }
}
