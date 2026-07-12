using Flowenter.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Flowenter.Parties.Models.PartyModels;

public abstract class PartyRelationship : EffectiveEntity
{
    [Required]
    public Guid? PartyRelationshipTypeId { get; set; }
    public PartyRelationshipType? PartyRelationshipType { get; set; }
}
