using QLNhanVien.src.Models.DTOs;

namespace QLNhanVien.src.Auth;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(AuthRegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthLoginResponse> LoginAsync(AuthLoginRequest request, CancellationToken cancellationToken = default);
    Task<bool> LogoutAsync();
}
