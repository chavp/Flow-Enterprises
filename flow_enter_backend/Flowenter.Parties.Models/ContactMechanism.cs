using Flowenter.Domain.Models;

namespace Flowenter.Parties.Models;

public abstract class ContactMechanism : BaseEntity
{
    public ContactMechanismType Type { get; set; }
}
