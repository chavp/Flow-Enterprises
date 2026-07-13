namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record EmploymentBranchDto
{
    public Guid BranchId { get; init; }
    public string BranchLegalName { get; init; } = string.Empty;
    public DateOnly FromDate { get; init; }
    public DateOnly ThruDate { get; init; }
}
