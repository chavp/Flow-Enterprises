using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Domain.Models;

public abstract class EffectiveEntity : BaseEntity    
{
    public DateOnly FromDateUtc { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly ThruDateUtc { get; set; } = DateOnly.MaxValue;
}
