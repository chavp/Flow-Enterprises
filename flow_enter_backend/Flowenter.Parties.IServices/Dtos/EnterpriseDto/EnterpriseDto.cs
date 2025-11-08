using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record EnterpriseDto
{
    public Guid OrganizationPartyId { get; init; }
    public Guid EnterpriseRoleId { get; init; }
    public string LegalName { get; init; }
    public string? Information { get; init; }
    public byte[] Logo { get; init; } = Array.Empty<byte>();
    public string? BrandName { get; init; }
    public string? Notes { get; init; }
    public Guid? LegalStructureId { get; init; }
    public string? BusinessRegistrationNumber { get; init; }
    public string? TaxId { get; init; }
    public int FiscalYearStartMonth { get; init; }
}
