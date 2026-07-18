using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Products.Models;

[Index(nameof(ProviderPartyId), nameof(Name), IsUnique = true)]
public class ProductFeatureCategory : BaseEntity
{
    public Guid? ProviderPartyId { get; set; }

    [Required, MaxLength(200)]
    public string? Name { get; set; }
}
