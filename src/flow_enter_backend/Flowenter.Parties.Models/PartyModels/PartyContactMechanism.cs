using Flowenter.Domain.Models;
using Flowenter.Parties.Models.ContactMechanismModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Flowenter.Parties.Models.PartyModels;

[Index(nameof(PartyId), nameof(PartyRoleTypeId), nameof(ContactMechanismId)
    , nameof(FromDate)
    , IsUnique = true)]
public sealed class PartyContactMechanism : EffectiveEntity, ITenantEnabled
{
    [Required]
    public Guid? TenantId { get; set; }

    [Required]
    public Guid? PartyId { get; set; }
    public Party? Party { get; set; }

    [Required]
    public Guid? PartyRoleTypeId { get; set; }
    public PartyRoleType? PartyRoleType { get; set; }

    [Required]
    public Guid? ContactMechanismId { get; set; }
    public ContactMechanism? ContactMechanism { get; set; }
}