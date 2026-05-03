using BaraoPsicologia.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaraoPsicologia.API;

internal static class ListQueryHelpers
{
    public static (int Page, int PageSize) ReadPaging(IQueryCollection query)
    {
        var page = int.TryParse(query["page"], out var p) ? Math.Max(1, p) : 1;
        var pageSize = int.TryParse(query["pageSize"], out var ps) ? Math.Clamp(ps, 1, 500) : 10;
        return (page, pageSize);
    }

    public static IQueryable<T> ApplyCreatedAtFilters<T>(this IQueryable<T> q, IQueryCollection query)
        where T : EntityBase
    {
        foreach (var key in query.Keys)
        {
            if (!key.EndsWith("Start", StringComparison.OrdinalIgnoreCase))
                continue;
            var prefix = key[..^5];
            var startKey = prefix + "Start";
            var endKey = prefix + "End";
            if (DateTime.TryParse(query[startKey], out var start))
                q = q.Where(e => e.CreatedAt >= start);
            if (DateTime.TryParse(query[endKey], out var end))
                q = q.Where(e => e.CreatedAt <= end);
        }

        return q;
    }
}
