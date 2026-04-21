using QLNhanVien.src.Models.DTOs;

namespace QLNhanVien.src.Services;

public interface IEmployeeService
{
    Task<PaginatedResponse<EmployeeDto>> GetAllAsync(EmployeeQueryRequest query, int currentUserId, CancellationToken cancellationToken = default);
    Task<EmployeeDto?> GetDetailAsync(int id, CancellationToken cancellationToken = default);
    Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, int currentUserId, CancellationToken cancellationToken = default);
    Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeRequest request, int currentUserId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int currentUserId, CancellationToken cancellationToken = default);
    Task<int> DeleteManyAsync(List<int> ids, int currentUserId, CancellationToken cancellationToken = default);
}
