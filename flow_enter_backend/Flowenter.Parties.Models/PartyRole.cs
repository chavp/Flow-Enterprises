using Flowenter.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.Models;

public abstract class PartyRole : EffectiveEntity
{
    public Guid TypeId { get; set; }
    public PartyRoleType Type { get; set; }

    public Guid PartyId { get; set; }
    public Party Party { get; set; }
}
