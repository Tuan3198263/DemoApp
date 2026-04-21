namespace QLNhanVien.src.Middleware;

public class JwtUserContextMiddleware
{
    private readonly RequestDelegate _next;

    public JwtUserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            context.Items["CurrentUserId"] = context.User.FindFirst("sub")?.Value;
            context.Items["CurrentUserName"] = context.User.Identity?.Name;
            context.Items["CurrentUserRole"] = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        }

        await _next(context);
    }
}
