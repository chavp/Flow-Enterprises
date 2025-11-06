using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Domain.Models;

public abstract class EffectiveEntity : BaseEntity    
{
    public DateTime FromDateUtc { get; set; }
    public DateTime? ThruDateUtc { get; set; }
}
