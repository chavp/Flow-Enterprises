using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.FacilityModels;

[Index(nameof(Code), IsUnique = true)]
public sealed class FacilityType : BaseEntity
{
    public const string Building = "BUILDING";
    public const string Floor = "FLOOR";
    public const string Room = "ROOM";
    public const string Bed = "BED";

    [Required, StringLength(100)]
    public string? Code { get; set; }

    [Required, StringLength(200)]
    public string? Name { get; set; }
}
