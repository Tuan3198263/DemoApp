using Microsoft.EntityFrameworkCore;
using QLNhanVien.src.Common.Utilities;
using QLNhanVien.src.Data;
using QLNhanVien.src.Models.DTOs;
using QLNhanVien.src.Models.Entities;

namespace QLNhanVien.src.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(AppDbContext dbContext, ILogger<EmployeeService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PaginatedResponse<EmployeeDto>> GetAllAsync(EmployeeQueryRequest query, int currentUserId, CancellationToken cancellationToken = default)
    {
        var normalized = PaginationFilterHelper.Normalize(query.Page, query.PageSize, 100);

        IQueryable<Employee> employeesQuery = _dbContext.Employees
            .AsNoTracking()
            .Where(e => e.DeletedAt == null);

        // Lọc theo keyword
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim().ToLowerInvariant();
            employeesQuery = employeesQuery.Where(e =>
                e.MaNhanVien.ToLower().Contains(keyword) ||
                e.TenNhanVien.ToLower().Contains(keyword));
        }

        // Lọc theo giới tính
        if (!string.IsNullOrWhiteSpace(query.GioiTinh))
        {
            employeesQuery = employeesQuery.Where(e => e.GioiTinh == query.GioiTinh);
        }

        // Sắp xếp
        employeesQuery = PaginationFilterHelper.ApplySorting(employeesQuery, query.SortBy, query.Descending);

        var totalItems = await employeesQuery.CountAsync(cancellationToken);
        var items = await employeesQuery
            .Skip((normalized.Page - 1) * normalized.PageSize)
            .Take(normalized.PageSize)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Lấy danh sách nhân viên: page={Page}, pageSize={PageSize}, total={Total}",
            normalized.Page, normalized.PageSize, totalItems);

        return new PaginatedResponse<EmployeeDto>
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

    public async Task<EmployeeDto?> GetDetailAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.DeletedAt == null, cancellationToken);

        return employee == null ? null : MapToDto(employee);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, int currentUserId, CancellationToken cancellationToken = default)
    {
        ValidateCreateRequest(request);

        var maNhanVien = request.MaNhanVien.Trim();
        var exists = await _dbContext.Employees
            .AnyAsync(e => e.MaNhanVien == maNhanVien && e.DeletedAt == null, cancellationToken);

        if (exists)
            throw new InvalidOperationException($"Mã nhân viên '{maNhanVien}' đã tồn tại");

        var employee = new Employee
        {
            MaNhanVien = maNhanVien,
            TenNhanVien = request.TenNhanVien.Trim(),
            GioiTinh = request.GioiTinh,
            BoPhan = request.BoPhan.Trim(),
            MucLuong = request.MucLuong,
            UpdatedBy = currentUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tạo nhân viên mới: id={Id}, maNhanVien={MaNhanVien}", employee.Id, employee.MaNhanVien);
        return MapToDto(employee);
    }

    public async Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeRequest request, int currentUserId, CancellationToken cancellationToken = default)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == id && e.DeletedAt == null, cancellationToken);

        if (employee == null)
            throw new InvalidOperationException($"Nhân viên id {id} không tồn tại");

        if (!string.IsNullOrWhiteSpace(request.TenNhanVien))
            employee.TenNhanVien = request.TenNhanVien.Trim();

        if (!string.IsNullOrWhiteSpace(request.GioiTinh))
            employee.GioiTinh = request.GioiTinh;

        if (!string.IsNullOrWhiteSpace(request.BoPhan))
            employee.BoPhan = request.BoPhan.Trim();

        if (request.MucLuong.HasValue)
            employee.MucLuong = request.MucLuong.Value;

        employee.UpdatedBy = currentUserId;
        employee.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cập nhật nhân viên: id={Id}, updatedBy={UpdatedBy}", employee.Id, currentUserId);
        return MapToDto(employee);
    }

    public async Task<bool> DeleteAsync(int id, int currentUserId, CancellationToken cancellationToken = default)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == id && e.DeletedAt == null, cancellationToken);

        if (employee == null)
        {
            _logger.LogWarning("Nhân viên id {Id} không tồn tại", id);
            return false;
        }

        employee.DeletedAt = DateTime.UtcNow;
        employee.UpdatedBy = currentUserId;
        employee.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Xóa nhân viên: id={Id}, deletedBy={DeletedBy}", employee.Id, currentUserId);
        return true;
    }

    public async Task<int> DeleteManyAsync(List<int> ids, int currentUserId, CancellationToken cancellationToken = default)
    {
        var validIds = ids.Where(i => i > 0).Distinct().ToList();
        if (validIds.Count == 0)
            return 0;

        var employees = await _dbContext.Employees
            .Where(e => validIds.Contains(e.Id) && e.DeletedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var employee in employees)
        {
            employee.DeletedAt = DateTime.UtcNow;
            employee.UpdatedBy = currentUserId;
            employee.UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Xóa {Count} nhân viên, deletedBy={DeletedBy}", employees.Count, currentUserId);
        return employees.Count;
    }

    private static EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            MaNhanVien = employee.MaNhanVien,
            TenNhanVien = employee.TenNhanVien,
            GioiTinh = employee.GioiTinh,
            BoPhan = employee.BoPhan,
            MucLuong = employee.MucLuong,
            UpdatedBy = employee.UpdatedBy,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };
    }

    private static void ValidateCreateRequest(CreateEmployeeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MaNhanVien))
            throw new InvalidOperationException("Mã nhân viên là bắt buộc");

        if (string.IsNullOrWhiteSpace(request.TenNhanVien))
            throw new InvalidOperationException("Tên nhân viên là bắt buộc");

        if (request.MucLuong < 0)
            throw new InvalidOperationException("Mức lương phải lớn hơn hoặc bằng 0");
    }
}
