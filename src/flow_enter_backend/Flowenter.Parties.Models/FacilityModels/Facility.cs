using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.FacilityModels;

[Index(nameof(Number))]
public class Facility : BaseEntity
{
    [Required, StringLength(100)]
    public string? Number { get; set; }

    [Required]
    public Guid? FacilityTypeId { get; set; }
    public FacilityType? FacilityType { get; set; }

    public Guid? PartOfId { get; set; }
    public Facility? PartOf { get; set; }
}
