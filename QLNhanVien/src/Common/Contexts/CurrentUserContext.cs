using System.Security.Claims;

namespace QLNhanVien.src.Common.Contexts;

public interface ICurrentUserContext
{
    int? CurrentUserId { get; }
}

public class HttpCurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? CurrentUserId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            if (httpContext.Items.TryGetValue("CurrentUserId", out var itemValue) &&
                int.TryParse(itemValue?.ToString(), out var itemUserId))
            {
                return itemUserId;
            }

            var claimValue = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? httpContext.User.FindFirstValue("sub");

            return int.TryParse(claimValue, out var userId) ? userId : null;
        }
    }
}