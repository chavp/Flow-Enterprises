using Flowenter.Domain.Models;
using Flowenter.Parties.Models.ContactMechanismModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.Models.PartyModels;

[Index(nameof(TenantId))]
public abstract class Party : BaseEntity, ITenantEnabled
{
    [Required]
    public Guid? TenantId { get; set; }

    [Required]
    public Guid? TypeId { get; set; }
    public PartyType? Type { get; set; }

    public List<ContactMechanism> ContactMechanisms { get; set; } = new();
    public List<FinancialAccount> FinancialAccounts { get; set; } = new();
}
