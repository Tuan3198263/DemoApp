namespace QLNhanVien.src.Models.DTOs;

/// <summary>
/// EmployeeDto - Thông tin nhân viên trả về API
/// </summary>
public class EmployeeDto
{
    public int Id { get; set; }
    public string MaNhanVien { get; set; } = string.Empty;
    public string TenNhanVien { get; set; } = string.Empty;
    public string GioiTinh { get; set; } = string.Empty;
    public string BoPhan { get; set; } = string.Empty;
    public decimal MucLuong { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// CreateEmployeeRequest - Tạo nhân viên mới
/// </summary>
public class CreateEmployeeRequest
{
    public string MaNhanVien { get; set; } = string.Empty;
    public string TenNhanVien { get; set; } = string.Empty;
    public string GioiTinh { get; set; } = "Nam";
    public string BoPhan { get; set; } = string.Empty;
    public decimal MucLuong { get; set; }
}

/// <summary>
/// UpdateEmployeeRequest - Cập nhật thông tin nhân viên
/// </summary>
public class UpdateEmployeeRequest
{
    public string? TenNhanVien { get; set; }
    public string? GioiTinh { get; set; }
    public string? BoPhan { get; set; }
    public decimal? MucLuong { get; set; }
}

/// <summary>
/// EmployeeQueryRequest - Yêu cầu query danh sách nhân viên
/// </summary>
public class EmployeeQueryRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public string? GioiTinh { get; set; }
    public string? BoPhan { get; set; }
    public string? SortBy { get; set; }
    public bool Descending { get; set; } = true;
}

/// <summary>
/// DeleteManyEmployeesRequest - Xóa nhiều nhân viên
/// </summary>
public class DeleteManyEmployeesRequest
{
    public List<int> EmployeeIds { get; set; } = new();
}
