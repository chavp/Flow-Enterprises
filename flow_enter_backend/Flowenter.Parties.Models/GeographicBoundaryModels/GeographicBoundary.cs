using Flowenter.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.Models.GeographicBoundaryModels;

public abstract class GeographicBoundary: BaseEntity
{
    public Guid? TypeId { get; set; }
    public GeographicBoundaryType? Type { get; set; }
}
