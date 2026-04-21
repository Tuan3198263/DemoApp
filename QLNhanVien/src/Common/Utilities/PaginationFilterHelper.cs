using System.Linq.Expressions;
using QLNhanVien.src.Models.Entities;

namespace QLNhanVien.src.Common.Utilities;

public static class PaginationFilterHelper
{
    public static (int Page, int PageSize) Normalize(int page, int pageSize, int maxPageSize = 100)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize < 1 ? 10 : pageSize;
        if (normalizedPageSize > maxPageSize)
        {
            normalizedPageSize = maxPageSize;
        }

        return (normalizedPage, normalizedPageSize);
    }

    public static IQueryable<User> ApplyUserFilter(IQueryable<User> query, string? keyword, string? status)
    {
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalizedKeyword = keyword.Trim().ToLowerInvariant();
            query = query.Where(u =>
                u.UserName.ToLower().Contains(normalizedKeyword) ||
                u.FullName.ToLower().Contains(normalizedKeyword) ||
                u.Email.ToLower().Contains(normalizedKeyword));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = NormalizeStatus(status);
            query = query.Where(u => u.Status == normalizedStatus);
        }

        return query;
    }

    public static string NormalizeStatus(string? status)
    {
        var normalized = status?.Trim().ToLowerInvariant();
        return normalized == UserStatuses.Employee ? UserStatuses.Employee : UserStatuses.Admin;
    }

    public static IQueryable<T> ApplySorting<T>(IQueryable<T> query, string? sortBy, bool descending = true) where T : BaseEntity
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return descending ? query.OrderByDescending(e => e.CreatedAt) : query.OrderBy(e => e.CreatedAt);
        }

        var param = Expression.Parameter(typeof(T), "x");
        var property = typeof(T).GetProperty(sortBy, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public);

        if (property == null)
        {
            return descending ? query.OrderByDescending(e => e.CreatedAt) : query.OrderBy(e => e.CreatedAt);
        }

        var propertyAccess = Expression.MakeMemberAccess(param, property);
        var orderByExpression = Expression.Lambda(propertyAccess, param);

        var methodName = descending ? "OrderByDescending" : "OrderBy";
        var resultExpression = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(T), property.PropertyType }, query.Expression, Expression.Quote(orderByExpression));

        return query.Provider.CreateQuery<T>(resultExpression);
    }
}
