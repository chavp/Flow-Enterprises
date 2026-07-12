using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Flowenter.Parties.Models.FacilityModels;

[Index(nameof(Name))]
public class Building : Facility
{
    [Required, StringLength(200)]
    public string? Name { get; set; }
}
