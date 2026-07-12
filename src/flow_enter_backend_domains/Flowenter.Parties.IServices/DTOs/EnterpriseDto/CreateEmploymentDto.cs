using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record CreateEmploymentDto
{
    [Required, StringLength(300)]
    public string FirstName { get; init; } = string.Empty;

    [StringLength(300)]
    public string? MiddleName { get; init; }

    [Required, StringLength(500)]
    public string LastName { get; init; } = string.Empty;

    [Required, MinLength(1)]
    public List<Guid> PartyRoleTypeIds { get; init; } = [];

    public Guid? LanguageId { get; init; }
    public DateOnly? DateOfBirth { get; init; }
}
