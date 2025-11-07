using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.GeographicBoundaryModels;

[Index(nameof(Name), IsUnique = true)]
public sealed class Country: GeographicBoundary
{
    [Required, StringLength(200)]
    public string? Name { get; set; }

    [Required, StringLength(2)]
    public string? IsoCode2 { get; set; }

    [Required, StringLength(3)]
    public string? IsoCode3 { get; set; }
}
