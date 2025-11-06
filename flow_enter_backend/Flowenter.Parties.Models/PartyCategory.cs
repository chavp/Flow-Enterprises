using Flowenter.Domain.Models;

namespace Flowenter.Parties.Models;

public sealed class PartyCategory : BaseEntity
{
    public string Name { get; set; }
    public PartyCategory? GroupBy { get; set; }
}
