using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.PartyModels;

public sealed class EnterpriseBranch : PartyRelationship
{
    protected EnterpriseBranch() { }
    public EnterpriseBranch(Guid enterpriseId, Guid branchId, Guid partyRelationshipTypeId)
    {
        EnterpriseId = enterpriseId;
        BranchId = branchId;
        PartyRelationshipTypeId = partyRelationshipTypeId;
    }

    [Required]
    public Guid? EnterpriseId { get; set; }
    public Enterprise? Enterprise { get; set; }

    [Required]
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }
}
