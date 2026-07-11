using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Flowenter.Parties.Models.PartyModels;

[Table("Employments")]
public class Employment : PartyRelationship
{
    protected Employment() { }
    public Employment(Guid employerId, Guid employeeId, Guid partyRelationshipTypeId)
    {
        EmployerId = employerId;
        EmployeeId = employeeId;
        PartyRelationshipTypeId = partyRelationshipTypeId;
    }

    [Required]
    public Guid? EmployerId { get; set; }
    public PartyRole? Employer { get; set; }

    [Required]
    public Guid? EmployeeId { get; set; }
    public PartyRole? Employee { get; set; }
}
