namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record EmploymentDto
{
    public Guid EmploymentId { get; init; }
    public Guid EmployerId { get; init; }
    public Guid EmployeePartyRoleId { get; init; }
    public Guid EmployeePartyId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string? MiddleName { get; init; }
    public string LastName { get; init; } = string.Empty;
    public string EmployeeFullName { get; init; } = string.Empty;
    public Guid PartyRoleTypeId { get; init; }
    public string PartyRoleTypeCode { get; init; } = string.Empty;
    public string PartyRoleTypeName { get; init; } = string.Empty;

    public DateOnly FromDate { get; init; }
    public DateOnly ThruDate { get; init; }

    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public ulong Revision { get; init; }
}
