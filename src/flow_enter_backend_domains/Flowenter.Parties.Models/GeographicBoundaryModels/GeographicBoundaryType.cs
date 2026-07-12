using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.GeographicBoundaryModels;

[Index(nameof(Code), IsUnique = true)]
public sealed class GeographicBoundaryType : BaseEntity
{
    public const string Country = "COUNTRY";
    public const string Province = "PROVINCE";
    public const string District = "DISTRICT";
    public const string Subdistrict = "SUBDISTRICT";

    [Required, StringLength(100)]
    public string? Code { get; set; }

    [Required, StringLength(200)]
    public string? Name { get; set; }
}
