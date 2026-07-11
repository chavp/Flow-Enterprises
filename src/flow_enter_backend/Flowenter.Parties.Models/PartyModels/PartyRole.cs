using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.PartyModels;

[Index(nameof(TypeId), nameof(PartyId), nameof(FromDate), IsUnique = true)]
public abstract class PartyRole : EffectiveEntity
{
    [Required]
    public Guid? TypeId { get; set; }
    public PartyRoleType? Type { get; set; }

    [Required]
    public Guid? PartyId { get; set; }
    public Party? Party { get; set; }
}
