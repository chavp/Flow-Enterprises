using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Flowenter.Parties.Models.FacilityModels;

[Index(nameof(Number))]
public class Room : Facility
{
    [Required, StringLength(100)]
    public string? Number { get; set; }

    public Guid? FloorId { get; set; }
    public Floor? Floor { get; set; }
}
