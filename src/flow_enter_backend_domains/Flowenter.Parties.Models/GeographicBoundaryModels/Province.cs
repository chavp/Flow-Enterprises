using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Flowenter.Parties.Models.GeographicBoundaryModels;

/// <summary>
/// https://en.wikipedia.org/wiki/Provinces_of_Thailand
/// </summary>
[Index(nameof(CountryId), nameof(Name), IsUnique = true)]
public sealed class Province : GeographicBoundary
{
    [Required]
    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }

    [Required, StringLength(200)]
    public string? Name { get; set; }

    [StringLength(3)]
    public string? Hs { get; set; }

    [StringLength(6)]
    public string? Iso { get; set; }

    [StringLength(5)]
    public string? Fips { get; set; }
}
