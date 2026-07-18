using Flowenter.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Products.Models;

public abstract class ProductFeatureApplicability : EffectiveEntity
{
    [Required]
    public Guid? ProductId { get; set; }
    public Product? Product { get; set; }

    [Required]
    public Guid? ProductFeatureId { get; set; }
    public ProductFeature? ProductFeature { get; set; }

    [Required, StringLength(200)]
    public string? ProductFeatureApplicabilityType { get; set; }

    public int Order { get; set; } = 0;
}

public class RequiredFeature : ProductFeatureApplicability;
public class StandardFeature : ProductFeatureApplicability;
public class OptionalFeature : ProductFeatureApplicability;
public class SelectableFeature : ProductFeatureApplicability;