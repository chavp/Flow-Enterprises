using Flowenter.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.Models.FacilityModels;

public class Floor : Facility
{
    public int Level { get; set; }

    public Guid? BuildingId { get; set; }
    public Building? Building { get; set; }
}
