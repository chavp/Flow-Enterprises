using Flowenter.Domain.Models;

namespace Flowenter.Parties.Models;

public sealed class PartyClassification : BaseEntity
{
    public PartyCategory Category { get; set; }
    public Party Party { get; set; }
}
