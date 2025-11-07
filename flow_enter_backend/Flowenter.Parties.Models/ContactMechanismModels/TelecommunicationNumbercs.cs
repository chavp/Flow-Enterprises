using Flowenter.Parties.Models.GeographicBoundaryModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Flowenter.Parties.Models.ContactMechanismModels;


[Table("TelecommunicationNumbers")]
[Index(nameof(CountryId), nameof(Number), IsUnique = true)]
public sealed class TelecommunicationNumber : ContactMechanism
{
    [Required]
    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }

    [Required, StringLength(15)]
    public string? Number { get; set; }

    [StringLength(3)]
    public string? CountryCode { get; set; }

    [StringLength(3)]
    public string? AreaCode { get; set; }

    [StringLength(300)]
    public string? AskForName { get; set; }
}
