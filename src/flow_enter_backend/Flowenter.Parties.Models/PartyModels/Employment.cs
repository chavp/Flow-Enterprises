using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Flowenter.Parties.Models.PartyModels;

[Table("Employments")]
public class Employment : PartyRelationship
{
    public Guid? EmployerId { get; set; }
    public PartyRole? Employer { get; set; }

    public Guid? EmployeeId { get; set; }
    public PartyRole? Employee { get; set; }
}
