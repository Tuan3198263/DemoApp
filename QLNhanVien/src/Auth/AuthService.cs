using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QLNhanVien.src.Common.Utilities;
using QLNhanVien.src.Data;
using QLNhanVien.src.Models.DTOs;
using QLNhanVien.src.Models.Entities;

namespace QLNhanVien.src.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<UserDto> RegisterAsync(AuthRegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
            throw new InvalidOperationException("Username is required");

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            throw new InvalidOperationException("Password must be at least 6 characters");

        var normalizedUserName = request.UserName.Trim().ToLowerInvariant();
        var exists = await _dbContext.Users.AnyAsync(u => u.UserName == normalizedUserName && u.DeletedAt == null, cancellationToken);
        if (exists)
            throw new InvalidOperationException("Username already exists");

        var user = new User
        {
            UserName = normalizedUserName,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password.Trim()),
            FullName = request.FullName.Trim(),
            Phone = request.Phone.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Status = PaginationFilterHelper.NormalizeStatus(request.Status)
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(user);
    }

    public async Task<AuthLoginResponse> LoginAsync(AuthLoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedUserName = request.UserName.Trim().ToLowerInvariant();
        var user = await _dbContext.Users.FirstOrDefaultAsync(
            u => u.UserName == normalizedUserName && u.DeletedAt == null,
            cancellationToken);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            throw new InvalidOperationException("Invalid username or password");

        var expirationDays = int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRATION_DAYS"), out var days)
            ? days
            : 7;

        var expiresAtUtc = DateTime.UtcNow.AddDays(expirationDays);
        var token = GenerateToken(user, expiresAtUtc);

        Console.WriteLine($"[LOGIN] user={user.UserName}, status={user.Status}, at={DateTime.UtcNow:O}");

        return new AuthLoginResponse
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAtUtc,
            User = MapToDto(user)
        };
    }

    public Task<bool> LogoutAsync()
    {
        // JWT stateless: client chỉ cần xóa token local.
        return Task.FromResult(true);
    }

    private string GenerateToken(User user, DateTime expiresAtUtc)
    {
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? _configuration["JWT_SECRET"]
            ?? throw new InvalidOperationException("JWT_SECRET is missing");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, user.Status)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FullName = user.FullName,
            Phone = user.Phone,
            Email = user.Email,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
