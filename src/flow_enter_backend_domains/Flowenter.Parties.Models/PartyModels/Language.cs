using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text;

namespace Flowenter.Parties.Models.PartyModels;

[Table("Languages")]
[Index(nameof(Code), IsUnique = true)]
public sealed class Language : BaseEntity
{
    public const string TH = "TH";
    public const string EN = "EN";

    
    [Required, StringLength(5)]
    public string? Code { get; set; }
}
