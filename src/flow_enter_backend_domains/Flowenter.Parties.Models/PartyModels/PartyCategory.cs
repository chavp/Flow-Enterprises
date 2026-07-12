using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.Models.PartyModels;

[Index(nameof(Name), IsUnique = true)]
public sealed class PartyCategory : BaseEntity
{
    [Required, StringLength(200)]
    public string? Name { get; set; }

    public Guid? GroupById { get; set; }
    public PartyCategory? GroupBy { get; set; }
}
