using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Flowenter.Parties.Models.PartyModels;

[Table("Customers")]
public sealed class Customer : PartyRole
{
}
