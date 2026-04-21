namespace QLNhanVien.src.Models.DTOs;

/// <summary>
/// UserDto - Trả về API response
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// CreateUserRequest - Tạo nhân viên mới
/// </summary>
public class CreateUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
}

/// <summary>
/// UpdateUserRequest - Cập nhật thông tin nhân viên
/// </summary>
public class UpdateUserRequest
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Department { get; set; }
    public string? Position { get; set; }
    public string? Address { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// PaginatedResponse - Response với pagination
/// </summary>
public class PaginatedResponse<T>
{
    public bool Success { get; set; }
    public List<T> Data { get; set; } = new();
    public PaginationInfo Pagination { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// PaginationInfo - Thông tin pagination
/// </summary>
public class PaginationInfo
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// ApiResponse - Response thống nhất
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, List<string>> Errors { get; set; } = new();
}

/// <summary>
/// ApiErrorResponse - Error response
/// </summary>
public class ApiErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, List<string>> Errors { get; set; } = new();
}
