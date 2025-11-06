using Flowenter.Domain.Models;

namespace Flowenter.Parties.Models;

public sealed class PartyContactMechanism: EffectiveEntity
{
        public Guid PartyId { get; set; }
        public Party Party { get; set; }
        public Guid PartyRoleTypeId { get; set; }
        public PartyRoleType PartyRoleType { get; set; }
        public Guid ContactMechanismId { get; set; }
        public ContactMechanism ContactMechanism { get; set; }
}
