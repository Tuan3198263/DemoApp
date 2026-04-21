using QLNhanVien.src.Models.DTOs;

namespace QLNhanVien.src.Services;

/// <summary>
/// IUserService - Interface định nghĩa contract cho User business logic
/// </summary>
public interface IUserService
{
    Task<PaginatedResponse<UserDto>> GetAllAsync(UserQueryRequest query, CancellationToken cancellationToken = default);
    Task<UserDto?> GetDetailAsync(int id, CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> DeleteManyAsync(List<int> ids, CancellationToken cancellationToken = default);
}
