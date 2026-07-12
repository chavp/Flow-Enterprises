using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.Models.PartyModels;

[Index(nameof(Code), IsUnique = true)]
public sealed class LegalStructure : BaseEntity
{
    public const string SoleProprietorship = "SOLE_PROPRIETORSHIP";
    public const string Partnership = "PARTNERSHIP";
    public const string Corporation = "CORPORATION";
    public const string LimitedLiabilityCompany = "LIMITED_LIABILITY_COMPANY";

    [Required, StringLength(100)]
    public string? Code { get; set; }

    [Required, StringLength(200)]
    public string? Name { get; set; }
}
