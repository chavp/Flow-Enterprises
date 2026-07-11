using Flowenter.Domain.Models;
using Flowenter.Parties.Models.PartyModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.FacilityModels;

public class FacilityRole : EffectiveEntity
{
    [Required]
    public Guid? FacilityRoleTypeId { get; set; }
    public FacilityRoleType? FacilityRoleType { get; set; }

    [Required]
    public Guid? PartyRoleTypeId { get; set; }
    public PartyRoleType? PartyRoleType { get; set; }

    [Required]
    public Guid? PartyId { get; set; }
    public Party? Party { get; set; }
}
