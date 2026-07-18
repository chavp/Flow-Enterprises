using Flowenter.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Products.Models;

public abstract class UnitOfMeasure : BaseEntity
{
    [Required, StringLength(200)]
    public string? UnitOfMeasureType { get; set; }

    [Required, StringLength(10)]
    public string? Abbreviation { get; set; }
}

public class TimeFrequencyMeasure : UnitOfMeasure
{
    public const string Day = "D";
    public const string Month = "M";
    public const string Year = "Y";
}

public class CurrencyMeasure : UnitOfMeasure
{
    public const string Baht = "THB";
    public const string USDollar = "USD";
}