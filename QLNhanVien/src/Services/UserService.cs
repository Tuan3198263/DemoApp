using Microsoft.EntityFrameworkCore;
using QLNhanVien.src.Data;
using QLNhanVien.src.Data.Repositories;
using QLNhanVien.src.Models.DTOs;
using QLNhanVien.src.Models.Entities;

namespace QLNhanVien.src.Services;

/// <summary>
/// UserService - Implements IUserService
/// Chứa toàn bộ business logic liên quan đến User
/// </summary>
public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IRepository<User> userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User with id {Id} not found", id);
                return null;
            }

            return MapToDto(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with id {Id}", id);
            throw;
        }
    }

    public async Task<List<UserDto>> GetAllActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _userRepository.FindAsync(u => u.IsActive && u.IsCurrentlyActive(), cancellationToken);
            return users.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all active users");
            throw;
        }
    }

    public async Task<PaginatedResponse<UserDto>> GetUsersPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting users: page={Page}, pageSize={PageSize}, search={Search}",
                pageNumber, pageSize, searchTerm ?? "none");

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Max page size

            Func<IQueryable<User>, IQueryable<User>>? filter = null;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filter = q => q.Where(u =>
                    u.FullName.Contains(searchTerm) ||
                    u.Email.Contains(searchTerm) ||
                    u.EmployeeCode.Contains(searchTerm)
                );
            }

            var (users, totalCount) = await _userRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                filter,
                cancellationToken
            );

            var dtos = users.Select(MapToDto).ToList();
            var totalPages = (totalCount + pageSize - 1) / pageSize;

            return new PaginatedResponse<UserDto>
            {
                Success = true,
                Data = dtos,
                Pagination = new PaginationInfo
                {
                    Page = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalCount,
                    TotalPages = totalPages
                } ?? new PaginationInfo(),
                Message = "Success"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged users");
            throw;
        }
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating user: {EmployeeCode} - {FullName}", request.EmployeeCode, request.FullName);

            // Validate
            ValidateCreateRequest(request);

            // Check duplicates
            if (await EmailExistsAsync(request.Email, cancellationToken))
                throw new InvalidOperationException($"Email '{request.Email}' already exists");

            if (await EmployeeCodeExistsAsync(request.EmployeeCode, cancellationToken))
                throw new InvalidOperationException($"Employee code '{request.EmployeeCode}' already exists");

            // Create entity
            var user = new User
            {
                FullName = request.FullName.Trim(),
                Email = request.Email.Trim().ToLower(),
                EmployeeCode = request.EmployeeCode.Trim(),
                PhoneNumber = request.PhoneNumber?.Trim() ?? "",
                Department = request.Department?.Trim() ?? "",
                Position = request.Position?.Trim() ?? "",
                Address = request.Address?.Trim() ?? "",
                StartDate = request.StartDate,
                IsActive = true
            };

            // Persist
            await _userRepository.AddAsync(user, cancellationToken);
            _logger.LogInformation("User created successfully: {Id} - {EmployeeCode}", user.Id, user.EmployeeCode);

            return MapToDto(user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error creating user: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            throw;
        }
    }

    public async Task<UserDto> UpdateUserAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user: {Id}", id);

            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                throw new InvalidOperationException($"User with id {id} not found");

            // Update fields
            if (!string.IsNullOrWhiteSpace(request.FullName))
                user.FullName = request.FullName.Trim();

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var newEmail = request.Email.Trim().ToLower();
                if (newEmail != user.Email && await EmailExistsAsync(newEmail, cancellationToken))
                    throw new InvalidOperationException($"Email '{newEmail}' already exists");
                user.Email = newEmail;
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber.Trim();

            if (!string.IsNullOrWhiteSpace(request.Department))
                user.Department = request.Department.Trim();

            if (!string.IsNullOrWhiteSpace(request.Position))
                user.Position = request.Position.Trim();

            if (!string.IsNullOrWhiteSpace(request.Address))
                user.Address = request.Address.Trim();

            if (request.IsActive.HasValue)
                user.IsActive = request.IsActive.Value;

            if (request.EndDate.HasValue)
                user.EndDate = request.EndDate.Value;

            // Persist
            await _userRepository.UpdateAsync(user, cancellationToken);
            _logger.LogInformation("User updated successfully: {Id}", id);

            return MapToDto(user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Validation error updating user {Id}: {Message}", id, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user: {Id}", id);

            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User with id {Id} not found", id);
                return false;
            }

            await _userRepository.DeleteAsync(user, cancellationToken);
            _logger.LogInformation("User deleted successfully: {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {Id}", id);
            throw;
        }
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var normalizedEmail = email.Trim().ToLower();
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
            return user != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email exists: {Email}", email);
            throw;
        }
    }

    public async Task<bool> EmployeeCodeExistsAsync(string employeeCode, CancellationToken cancellationToken = default)
    {
        try
        {
            var code = employeeCode.Trim();
            var user = await _userRepository.FirstOrDefaultAsync(u => u.EmployeeCode == code, cancellationToken);
            return user != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking employee code exists: {Code}", employeeCode);
            throw;
        }
    }

    // Private Methods
    private static UserDto MapToDto(User user) =>
        new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            EmployeeCode = user.EmployeeCode,
            PhoneNumber = user.PhoneNumber,
            Department = user.Department,
            Position = user.Position,
            Address = user.Address,
            IsActive = user.IsActive,
            StartDate = user.StartDate,
            EndDate = user.EndDate,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

    private static void ValidateCreateRequest(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new InvalidOperationException("Full name is required");

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
            throw new InvalidOperationException("Valid email is required");

        if (string.IsNullOrWhiteSpace(request.EmployeeCode))
            throw new InvalidOperationException("Employee code is required");

        if (request.StartDate == default)
            throw new InvalidOperationException("Start date is required");
    }
}
