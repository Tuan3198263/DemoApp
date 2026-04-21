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
}
