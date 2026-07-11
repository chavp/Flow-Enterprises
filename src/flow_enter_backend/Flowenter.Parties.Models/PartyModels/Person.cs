using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flowenter.Parties.Models.PartyModels;

[Table("People")]
[Index(nameof(FirstName), nameof(MiddleName), nameof(LastName), IsUnique = true)]
public class Person : Party
{
    [Required, StringLength(300)]
    public string? FirstName { get; set; }

    [Required, StringLength(300)]
    public string? MiddleName { get; set; }

    [Required, StringLength(500)]
    public string? LastName { get; set; }

    public DateTime? BirthDate { get; set; }
}
