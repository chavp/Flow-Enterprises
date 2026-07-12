using Flowenter.Domain.Models;
using Flowenter.Parties.Models.ContactMechanismModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.Models.PartyModels;

public abstract class Party : BaseEntity
{

    [Required]
    public Guid? TypeId { get; set; }
    public PartyType? Type { get; set; }

    public List<ContactMechanism> ContactMechanisms { get; set; } = new();
    public List<FinancialAccount> FinancialAccounts { get; set; } = new();
}
