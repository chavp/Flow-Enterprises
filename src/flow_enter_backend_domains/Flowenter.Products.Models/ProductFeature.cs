using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Products.Models;

[Index(nameof(Code), nameof(ProviderPartyId), IsUnique = true)]
public abstract  class ProductFeature : BaseEntity
{
    public Guid? ProviderPartyId { get; set; }

    [Required, StringLength(300)]
    public string? Title { get; set; }

    [Required, StringLength(100)]
    public string? Code { get; set; }
}
