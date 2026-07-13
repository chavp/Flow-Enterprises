using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Flowenter.Parties.Models.PartyModels;

[Index(nameof(EnterpriseId), nameof(EmployeeId), nameof(Number), IsUnique = true)]
public sealed class Employment : PartyRelationship
{
    protected Employment() { }
    public Employment(Guid enterpriseId, Guid employeeId, Guid partyRelationshipTypeId)
    {
        EnterpriseId = enterpriseId;
        EmployeeId = employeeId;
        PartyRelationshipTypeId = partyRelationshipTypeId;
    }

    [Required]
    public Guid? EnterpriseId { get; set; }
    public Enterprise? Enterprise { get; set; }

    [Required]
    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    [Required, StringLength(32)]
    public string? Number { get; set; } = Guid.NewGuid().ToString("N");
}
