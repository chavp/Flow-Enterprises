using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Domain.Models;

public abstract class EffectiveEntity : BaseEntity    
{
    public DateOnly FromDate { get; set; }
    public DateOnly? ThruDate { get; set; }
}
