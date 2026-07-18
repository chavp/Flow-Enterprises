namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record EnterpriseBranchDto
{
    public Guid EnterpriseBranchId { get; init; }
    public Guid EnterpriseId { get; init; }
    public Guid BranchId { get; init; }
    public Guid BranchPartyId { get; init; }
    public string BranchLegalName { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public ulong Revision { get; init; }
}
