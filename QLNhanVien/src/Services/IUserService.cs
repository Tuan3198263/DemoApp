using QLNhanVien.src.Models.DTOs;

namespace QLNhanVien.src.Services;

/// <summary>
/// IUserService - Interface định nghĩa contract cho User business logic
/// </summary>
public interface IUserService
{
    // Query
    Task<UserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<UserDto>> GetAllActiveUsersAsync(CancellationToken cancellationToken = default);
    Task<PaginatedResponse<UserDto>> GetUsersPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    // Modification
    Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserDto> UpdateUserAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken = default);

    // Utility
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmployeeCodeExistsAsync(string employeeCode, CancellationToken cancellationToken = default);
}
