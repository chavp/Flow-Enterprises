using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.PartyModels;

[Index(nameof(Code), IsUnique = true)]
public sealed class GenderType : BaseEntity
{
    public const string Male = "MALE";
    public const string Female = "FEMALE";

    [Required, StringLength(20)]
    public string? Code { get; set; }

    [Required, StringLength(100)]
    public string? Name { get; set; }
}
