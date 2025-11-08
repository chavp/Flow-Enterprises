using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.IServices.Dtos.EnterpriseDto;

public record EnterpriseDto
{
    public Guid OrganizationPartyId { get; init; }
    public Guid EnterpriseRoleId { get; init; }
    public Guid TenantId { get; init; }
    public string LegalName { get; init; }
    public string? Information { get; init; }
    public byte[] Logo { get; init; } = Array.Empty<byte>();
    public string? BrandName { get; init; }
    public string? Notes { get; init; }
    public Guid? LegalStructureId { get; init; }
    public string? BusinessRegistrationNumber { get; init; }
    public string? TaxId { get; init; }
    public int FiscalYearStartMonth { get; init; }

    public DateOnly FromDate { get; set; }
    public DateOnly ThruDate { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    [Required, StringLength(300)]
    public string? CreatedBy { get; set; }

    [StringLength(300)]
    public string? UpdatedBy { get; set; }
    public ulong Revision { get; set; }
}