namespace Flowenter.Parties.Models;

public sealed class Enterprise : PartyRole
{
    public string? Information { get; set; }
    public byte[]? Logo { get; set; }
    public string LegalName { get; set; }
    public string? BrandName { get; set; }
    public string? Notes { get; set; }

    public LegalStructure? LegalStructure { get; set; }
}
