using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.FacilityModels;

public class Facility : BaseEntity
{

    [Required]
    public Guid? FacilityTypeId { get; set; }
    public FacilityType? FacilityType { get; set; }

    public Guid? PartOfId { get; set; }
    public Facility? PartOf { get; set; }
}
