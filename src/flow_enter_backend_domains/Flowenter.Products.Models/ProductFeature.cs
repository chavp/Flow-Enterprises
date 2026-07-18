using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Products.Models;

[Index(nameof(ProviderPartyId),
    nameof(ProductFeatureType), 
    nameof(ProductFeatureCategoryId),
    nameof(Code),
    IsUnique = true)]
public abstract  class ProductFeature : BaseEntity
{
    public Guid? ProviderPartyId { get; set; }

    [Required, StringLength(200)]
    public string? ProductFeatureType { get; set; }

    [Required]
    public Guid? ProductFeatureCategoryId { get; set; }
    public ProductFeatureCategory? ProductFeatureCategory { get; set; }


    [Required, StringLength(100)]
    public string? Code { get; set; }


    [Required, StringLength(300)]
    public string? Title { get; set; }

}
