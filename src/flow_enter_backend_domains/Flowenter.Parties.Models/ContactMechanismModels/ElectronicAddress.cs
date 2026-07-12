using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flowenter.Parties.Models.ContactMechanismModels;

[Table("ElectronicAddresses")]
[Index(nameof(Address))]
public sealed class ElectronicAddress : ContactMechanism
{
    [Required, StringLength(320)]
    public string? Address { get; set; }
}
