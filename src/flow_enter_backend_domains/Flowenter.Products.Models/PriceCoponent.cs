using Flowenter.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Products.Models;

public abstract class PriceCoponent: EffectiveEntity
{
    [Required, StringLength(200)]
    public string? PriceCoponentType { get; set; }

    public decimal? Price { get; set; }
    public decimal? Percent { get; set; }

    public Guid? UnitOfMeasureId { get; set; }
    public UnitOfMeasure? UnitOfMeasure { get; set; }

    public Guid? SpecifiedForPartyId { get; set; }
}

public class BasePrice: PriceCoponent
{
    
}

public class RecurringCharge : PriceCoponent
{
    [Required]
    public Guid? TimeFrequencyMeasureId { get; set; }
    public TimeFrequencyMeasure? TimeFrequencyMeasure { get; set; }
}