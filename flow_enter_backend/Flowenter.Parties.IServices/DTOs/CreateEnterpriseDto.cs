namespace Flowenter.Parties.IServices.DTOs;

public record CreateEnterpriseDto(string LegalName)
{
    public string? Information { get; set; }
    public byte[] Logo { get; set; } = [];
    public string? BrandName { get; set; }
    public string? Notes { get; set; }
    public Guid? LegalStructureId { get; set; }
    public string? BusinessRegistrationNumber { get; set; }
    public string? TaxId { get; set; }
    public int FiscalYearStartMonth { get; set; }
}
