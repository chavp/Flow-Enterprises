using Flowenter.Domain.Models;
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
}
