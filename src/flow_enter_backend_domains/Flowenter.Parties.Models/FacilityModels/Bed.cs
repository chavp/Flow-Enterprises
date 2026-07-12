using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.FacilityModels;

[Index(nameof(Number))]
public class Bed : Facility
{
    [Required, StringLength(100)]
    public string? Number { get; set; }

    public Guid? RoomId { get; set; }
    public Room? Room { get; set; }
}
