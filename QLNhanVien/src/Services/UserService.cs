using Microsoft.EntityFrameworkCore;
using QLNhanVien.src.Common.Utilities;
using QLNhanVien.src.Data;
using QLNhanVien.src.Models.DTOs;
using QLNhanVien.src.Models.Entities;

namespace QLNhanVien.src.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext dbContext, ILogger<UserService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PaginatedResponse<UserDto>> GetAllAsync(UserQueryRequest query, CancellationToken cancellationToken = default)
    {
        var normalized = PaginationFilterHelper.Normalize(query.Page, query.PageSize, 100);

        IQueryable<User> usersQuery = _dbContext.Users.AsNoTracking().Where(u => u.DeletedAt == null);
        usersQuery = PaginationFilterHelper.ApplyUserFilter(usersQuery, query.Keyword, query.Status);
        usersQuery = usersQuery.OrderByDescending(u => u.CreatedAt);

        var totalItems = await usersQuery.CountAsync(cancellationToken);
        var items = await usersQuery
            .Skip((normalized.Page - 1) * normalized.PageSize)
            .Take(normalized.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<UserDto>
        {
            Success = true,
            Data = items.Select(MapToDto).ToList(),
            Pagination = new PaginationInfo
            {
                Page = normalized.Page,
                PageSize = normalized.PageSize,
                TotalItems = totalItems,
                TotalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)normalized.PageSize)
            },
            Message = "Success"
        };
    }

    public async Task<UserDto?> GetDetailAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null, cancellationToken);

        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCreateRequest(request);

        var normalizedUserName = request.UserName.Trim().ToLowerInvariant();
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedStatus = PaginationFilterHelper.NormalizeStatus(request.Status);

        var usernameExists = await _dbContext.Users.AnyAsync(u => u.UserName == normalizedUserName && u.DeletedAt == null, cancellationToken);
        if (usernameExists)
            throw new InvalidOperationException("Username already exists");

        var emailExists = !string.IsNullOrWhiteSpace(normalizedEmail)
            && await _dbContext.Users.AnyAsync(u => u.Email == normalizedEmail && u.DeletedAt == null, cancellationToken);
        if (emailExists)
            throw new InvalidOperationException("Email already exists");

        var user = new User
        {
            UserName = normalizedUserName,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password.Trim()),
            FullName = request.FullName.Trim(),
            Phone = request.Phone.Trim(),
            Email = normalizedEmail,
            Status = normalizedStatus,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created user id={Id}, username={UserName}", user.Id, user.UserName);
        return MapToDto(user);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null, cancellationToken);
        if (user == null)
        {
            return false;
        }

        user.DeletedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> DeleteManyAsync(List<int> ids, CancellationToken cancellationToken = default)
    {
        var validIds = ids.Where(i => i > 0).Distinct().ToList();
        if (validIds.Count == 0)
        {
            return 0;
        }

        var users = await _dbContext.Users
            .Where(u => validIds.Contains(u.Id) && u.DeletedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var user in users)
        {
            user.DeletedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return users.Count;
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

    private static void ValidateCreateRequest(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
            throw new InvalidOperationException("Username is required");

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            throw new InvalidOperationException("Password must be at least 6 characters");

        if (!string.IsNullOrWhiteSpace(request.Email) && !request.Email.Contains('@'))
            throw new InvalidOperationException("Email is invalid");
    }
}
