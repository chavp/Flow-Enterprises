using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.Models.ContactMechanismModels;

[Index(nameof(Address))]
public sealed class ElectronicAddress : ContactMechanism
{
    [Required, StringLength(320)]
    public string? Address { get; set; }
}
