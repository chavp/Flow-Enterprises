using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.Models.ContactMechanismModels;

[Index(nameof(Code), IsUnique = true)]
public sealed class ContactMechanismType: BaseEntity
{
    public const string Email = "EMAIL";
    public const string Website = "WEBSITE";
    public const string Phone = "PHONE";
    public const string Fax = "FAX";

    [Required, StringLength(100)]
    public string? Code { get; set; }

    [Required, StringLength(200)]
    public string? Name { get; set; }
}
