using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.PartyModels;

[Index(nameof(OwnerId), nameof(Number), IsUnique = true)]
public sealed class FinancialAccount: BaseEntity
{
    [Required, StringLength(200)]
    public string? Name { get; set; }

    [Required, StringLength(50)]
    public string? Number { get; set; }

    [Required, StringLength(10)]
    public string? Type { get; set; }

    [Required, StringLength(5)]
    public string? Currency { get; set; }

    public decimal Balance { get; set; }

    [Required]
    public Guid? OwnerId { get; set; }
    public Party? Owner { get; set; }
}
