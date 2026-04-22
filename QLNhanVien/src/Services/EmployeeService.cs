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

    public async Task<PaginatedResponse<EmployeeDto>> GetAllAsync(EmployeeQueryRequest query, CancellationToken cancellationToken = default)
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

        if (!string.IsNullOrWhiteSpace(query.BoPhan))
        {
            var boPhan = query.BoPhan.Trim().ToLowerInvariant();
            employeesQuery = employeesQuery.Where(e => e.BoPhan.ToLower().Contains(boPhan));
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

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default)
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
            NgaySinh = request.NgaySinh,
            GioiTinh = NormalizeGender(request.GioiTinh),
            BoPhan = request.BoPhan.Trim(),
            MucLuong = request.MucLuong
        };

        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tạo nhân viên mới: id={Id}, maNhanVien={MaNhanVien}", employee.Id, employee.MaNhanVien);
        return MapToDto(employee);
    }

    public async Task<List<EmployeeDto>> CreateManyAsync(List<CreateEmployeeRequest> requests, CancellationToken cancellationToken = default)
    {
        if (requests.Count == 0)
        {
            throw new InvalidOperationException("Danh sách nhân viên không được để trống");
        }

        foreach (var request in requests)
        {
            ValidateCreateRequest(request);
        }

        var normalizedCodes = requests
            .Select(request => request.MaNhanVien.Trim())
            .ToList();

        var duplicateInBody = normalizedCodes
            .GroupBy(code => code, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1)?.Key;

        if (!string.IsNullOrWhiteSpace(duplicateInBody))
        {
            throw new InvalidOperationException($"Mã nhân viên '{duplicateInBody}' bị trùng trong danh sách gửi lên");
        }

        var existingCodes = await _dbContext.Employees
            .Where(employee => normalizedCodes.Contains(employee.MaNhanVien) && employee.DeletedAt == null)
            .Select(employee => employee.MaNhanVien)
            .ToListAsync(cancellationToken);

        if (existingCodes.Count > 0)
        {
            throw new InvalidOperationException($"Mã nhân viên đã tồn tại: {string.Join(", ", existingCodes)}");
        }

        var employees = requests.Select(request => new Employee
        {
            MaNhanVien = request.MaNhanVien.Trim(),
            TenNhanVien = request.TenNhanVien.Trim(),
            NgaySinh = request.NgaySinh,
            GioiTinh = NormalizeGender(request.GioiTinh),
            BoPhan = request.BoPhan.Trim(),
            MucLuong = request.MucLuong
        }).ToList();

        _dbContext.Employees.AddRange(employees);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tạo nhiều nhân viên: {Count}", employees.Count);
        return employees.Select(MapToDto).ToList();
    }

    public async Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == id && e.DeletedAt == null, cancellationToken);

        if (employee == null)
            throw new InvalidOperationException($"Nhân viên id {id} không tồn tại");

        if (!string.IsNullOrWhiteSpace(request.TenNhanVien))
            employee.TenNhanVien = request.TenNhanVien.Trim();

        if (request.NgaySinh.HasValue)
            employee.NgaySinh = request.NgaySinh.Value;

        if (!string.IsNullOrWhiteSpace(request.GioiTinh))
            employee.GioiTinh = NormalizeGender(request.GioiTinh);

        if (!string.IsNullOrWhiteSpace(request.BoPhan))
            employee.BoPhan = request.BoPhan.Trim();

        if (request.MucLuong.HasValue)
            employee.MucLuong = request.MucLuong.Value;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cập nhật nhân viên: id={Id}", employee.Id);
        return MapToDto(employee);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == id && e.DeletedAt == null, cancellationToken);

        if (employee == null)
        {
            _logger.LogWarning("Nhân viên id {Id} không tồn tại", id);
            return false;
        }

        employee.DeletedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Xóa nhân viên: id={Id}", employee.Id);
        return true;
    }

    public async Task<int> DeleteManyAsync(List<int> ids, CancellationToken cancellationToken = default)
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
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Xóa {Count} nhân viên", employees.Count);
        return employees.Count;
    }

    private static EmployeeDto MapToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            MaNhanVien = employee.MaNhanVien,
            TenNhanVien = employee.TenNhanVien,
            NgaySinh = employee.NgaySinh,
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

        if (request.NgaySinh == default)
            throw new InvalidOperationException("Ngày sinh là bắt buộc");

        if (request.MucLuong < 0)
            throw new InvalidOperationException("Mức lương phải lớn hơn hoặc bằng 0");

    }

    private static string NormalizeGender(string gender)
    {
        var normalized = gender.Trim();
        return normalized switch
        {
            GioiTinhConstants.Nam => GioiTinhConstants.Nam,
            GioiTinhConstants.Nu => GioiTinhConstants.Nu,
            GioiTinhConstants.Khac => GioiTinhConstants.Khac,
            _ => throw new InvalidOperationException("Giới tính chỉ nhận Nam, Nữ hoặc Khác")
        };
    }
}
