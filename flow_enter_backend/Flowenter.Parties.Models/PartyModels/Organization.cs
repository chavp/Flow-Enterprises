using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.PartyModels;

[Index(nameof(Name), IsUnique = true)]
public class Organization : Party
{
    [Required, StringLength(200)]
    public string? Name { get; set; }
}
