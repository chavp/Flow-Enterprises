using Flowenter.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.Models;

public sealed class FinancialAccount: BaseEntity
{
    public string Name { get; set; }
    public string AccountNumber { get; set; }
    public string AccountType { get; set; }
    public string Currency { get; set; }
    public decimal Balance { get; set; }
    public Guid PartyId { get; set; }
    public Party Party { get; set; }
}
