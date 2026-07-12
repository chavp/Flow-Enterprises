using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record CreateEnterpriseBranchDto
{
    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;
}
