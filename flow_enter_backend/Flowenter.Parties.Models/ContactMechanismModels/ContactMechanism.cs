using Flowenter.Domain.Models;
using Flowenter.Parties.Models.PartyModels;
using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.Models.ContactMechanismModels;

public abstract class ContactMechanism : BaseEntity
{
    [Required]
    public Guid? TypeId { get; set; }
    public ContactMechanismType? Type { get; set; }

    public List<Party> Parties { get; set; } = new();
}
