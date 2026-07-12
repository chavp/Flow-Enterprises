using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Domain.Models;

public static class Tenants
{
    public static readonly Guid DefaultTenantId = Guid.Parse("00000000-0000-0000-0000-000000000000");
    public static readonly Guid PublicTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static readonly IReadOnlyList<Guid> All = new List<Guid>()
    {
        DefaultTenantId,
        PublicTenantId
    };
}
