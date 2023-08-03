using System;
using System.Linq;

namespace Remotely.Server.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<T> Apply<T>(this IQueryable<T> query, Action<IQueryable<T>>? queryBuilder)
    {
        if (queryBuilder is null)
        {
            return query;
        }

        queryBuilder.Invoke(query);
        return query;
    }
}
