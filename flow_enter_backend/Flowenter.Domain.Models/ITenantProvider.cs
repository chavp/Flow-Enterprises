using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Domain.Models;

public interface ITenantProvider
{
    Guid? GetTenantId();
    string GetTenantConnectionString();
}
