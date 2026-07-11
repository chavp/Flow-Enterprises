using Flowenter.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.Models.PartyModels;

public sealed class PartyClassification : BaseEntity
{
    [Required]
    public Guid? PartyId { get; set; }
    public Party? Party { get; set; }

    [Required]
    public Guid? CategoryId { get; set; }
    public PartyCategory? Category { get; set; }
}
