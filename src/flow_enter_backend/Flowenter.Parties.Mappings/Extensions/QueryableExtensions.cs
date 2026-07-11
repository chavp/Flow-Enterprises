using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Parties.Mappings.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ForTenant<T>(this DbSet<T> dbSet, Guid? tenantId)
        where T : class, ITenantEnabled
    {
        return dbSet.Where(e => e.TenantId == tenantId);
    }

    public static IQueryable<T> Effective<T>(this DbSet<T> dbSet)
        where T : EffectiveEntity
    {
        var toDay = DateOnly.FromDateTime(DateTime.UtcNow);
        return dbSet.Where(e => e.FromDate <= toDay && toDay <= e.ThruDate);
    }

}
