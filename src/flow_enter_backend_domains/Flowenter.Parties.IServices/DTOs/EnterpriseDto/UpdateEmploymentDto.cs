using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record UpdateEmploymentDto
{
    [Required, StringLength(32)]
    public string EmploymentNumber { get; init; } = string.Empty;

    [Required, StringLength(300)]
    public string FirstName { get; init; } = string.Empty;

    [StringLength(300)]
    public string? MiddleName { get; init; }

    [Required, StringLength(500)]
    public string LastName { get; init; } = string.Empty;

    [Required, MinLength(1)]
    public List<Guid> PartyRoleTypeIds { get; init; } = [];

    public List<Guid> BranchIds { get; init; } = [];
}
