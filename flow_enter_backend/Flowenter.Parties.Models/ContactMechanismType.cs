using Flowenter.Domain.Models;

namespace Flowenter.Parties.Models;

public sealed class ContactMechanismType: BaseEntity
{
    public const string Email = "EMAIL";
    public const string Website = "WEBSITE";
    public const string Phone = "PHONE";
    public const string Fax = "FAX";
}
