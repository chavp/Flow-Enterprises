using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Flowenter.Products.Models;

[Index(nameof(ProviderPartyId), nameof(Name), IsUnique = true)]
public abstract class Product : BaseEntity
{
    public Guid? ProviderPartyId { get; set; }

    [Required, MaxLength(200)]
    public string? Name { get; set; }

    public DateOnly? ReleaseDate { get; set; }
    public DateOnly? DiscontinuedDate { get; set; }
    public DateOnly? SupportDiscontinuedDate { get; set; }

    public byte[] CoverImage { get; set; }
    [MaxLength(200)]
    public string? CoverImageName { get; set; }

}