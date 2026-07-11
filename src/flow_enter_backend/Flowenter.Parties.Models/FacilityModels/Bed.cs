using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.FacilityModels;

public class Bed : Facility
{
    public Guid? RoomId { get; set; }
    public Room? Room { get; set; }
}
