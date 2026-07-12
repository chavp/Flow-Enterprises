using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.GeographicBoundaryModels;

[Index(nameof(DistrictId), nameof(Name), IsUnique = true)]
public sealed class Subdistrict : GeographicBoundary
{
    [Required]
    public Guid? DistrictId { get; set; }
    public District? District { get; set; }

    [StringLength(10)]
    public string? PrefixName { get; set; }

    [StringLength(5)]
    public string? PrefixShortName { get; set; }

    [Required, StringLength(200)]
    public string? Name { get; set; }

    [StringLength(10)]
    public string? PostalCode { get; set; }

}
