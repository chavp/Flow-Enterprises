using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Flowenter.Parties.Models.GeographicBoundaryModels;

[Table("Countries")]
[Index(nameof(Name), IsUnique = true)]
public sealed class Country: GeographicBoundary
{
    [Required, StringLength(200)]
    public string? Name { get; set; }

    [Required, StringLength(200)]
    public string? Nationality { get; set; }

    [Column("Numerric")]
    public int? Numeric { get; set; }

    [Required, StringLength(2)]
    public string? IsoCode2 { get; set; }

    [Required, StringLength(3)]
    public string? IsoCode3 { get; set; }
}
