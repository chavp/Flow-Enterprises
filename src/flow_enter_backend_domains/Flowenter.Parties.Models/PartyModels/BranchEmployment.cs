using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.PartyModels;

[Index(nameof(BranchId), nameof(EmployeeId), IsUnique = true)]
public sealed class BranchEmployment : PartyRelationship
{
    protected BranchEmployment() { }
    public BranchEmployment(Guid branchId, Guid employeeId, Guid partyRelationshipTypeId)
    {
        BranchId = branchId;
        EmployeeId = employeeId;
        PartyRelationshipTypeId = partyRelationshipTypeId;
    }

    [Required]
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    [Required]
    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }

}
