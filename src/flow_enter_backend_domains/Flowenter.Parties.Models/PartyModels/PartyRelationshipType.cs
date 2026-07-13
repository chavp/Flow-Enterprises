using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Flowenter.Parties.Models.PartyModels;

[Index(nameof(Code), IsUnique = true)]
public sealed class PartyRelationshipType : BaseEntity
{
    public const string Employment = "EMPLOYMENT";
    public const string EnterpriseBranch = "ENTERPRISE_BRANCH";
    public const string BranchEmployment = "BRANCH_EMPLOYMENT";

    [Required, StringLength(100)]
    public string? Code { get; set; }

    [Required, StringLength(200)]
    public string? Name { get; set; }
}
