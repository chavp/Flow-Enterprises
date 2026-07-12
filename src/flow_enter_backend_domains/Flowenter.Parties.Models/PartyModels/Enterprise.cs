using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Flowenter.Parties.Models.PartyModels;

[Index(nameof(LegalName), IsUnique = true)]
public sealed class Enterprise : PartyRole
{
    [Required, StringLength(200)]
    public string? LegalName { get; set; }

    [StringLength(500)]
    public string? Information { get; set; }
    public byte[]? Logo { get; set; }

    [StringLength(100)]
    public string? BrandName { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [Required]
    public Guid? LegalStructureId { get; set; }
    public LegalStructure? LegalStructure { get; set; }

    [StringLength(50)]
    public string? BusinessRegistrationNumber { get; set; }

    [StringLength(50)]
    public string? TaxId { get; set; }

    public int FiscalYearStartMonth { get; set; }
}
