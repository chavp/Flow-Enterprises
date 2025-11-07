using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Domain.Models;

public abstract class EffectiveEntity : BaseEntity    
{
    public DateOnly FromDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly ThruDate { get; set; } = DateOnly.FromDateTime(DateTime.MaxValue);
}
