using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Domain.Models;

public interface ITenantEnabled
{
    Guid? TenantId { get; set; }
}
