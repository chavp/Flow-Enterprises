using Flowenter.Domain.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Parties.Models.PartyModels;

[Index(nameof(Code), IsUnique = true)]
public sealed class PartyRoleType: BaseEntity   
{
    public const string Enterprise = "ENTERPRISE";
    public const string Customer = "CUSTOMER";

    [Required, StringLength(100)]
    public string? Code { get; set; }

    [Required, StringLength(200)]
    public string? Name { get; set; }
}
