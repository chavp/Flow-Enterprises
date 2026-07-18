using Flowenter.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Products.Models;

public class ProductFeatureCategory : BaseEntity
{
    public Guid? ProviderPartyId { get; set; }

    [Required, MaxLength(200)]
    public string? Name { get; set; }
}
