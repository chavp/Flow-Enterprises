using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.GeographicBoundaryModels;

[Index(nameof(ProvinceId), nameof(Name), IsUnique = true)]
public sealed class District : GeographicBoundary
{
    [Required]
    public Guid? ProvinceId { get; set; }
    public Province? Province { get; set; }

    [StringLength(10)]
    public string? PrefixName { get; set; }

    [StringLength(5)]
    public string? PrefixShortName { get; set; }

    [Required, StringLength(200)]
    public string? Name { get; set; }

    [StringLength(10)]
    public string? PostalCode { get; set; }

}
