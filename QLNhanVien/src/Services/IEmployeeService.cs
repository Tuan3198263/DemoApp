using QLNhanVien.src.Models.DTOs;

namespace QLNhanVien.src.Services;

public interface IEmployeeService
{
    Task<PaginatedResponse<EmployeeDto>> GetAllAsync(EmployeeQueryRequest query, CancellationToken cancellationToken = default);
    Task<EmployeeDto?> GetDetailAsync(int id, CancellationToken cancellationToken = default);
    Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default);
    Task<List<EmployeeDto>> CreateManyAsync(List<CreateEmployeeRequest> requests, CancellationToken cancellationToken = default);
    Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> DeleteManyAsync(List<int> ids, CancellationToken cancellationToken = default);
}
