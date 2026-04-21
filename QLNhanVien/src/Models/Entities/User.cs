namespace QLNhanVien.src.Models.Entities;

/// <summary>
/// Entity User - Nhân viên
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Tên nhân viên
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Mã nhân viên
    /// </summary>
    public string EmployeeCode { get; set; } = string.Empty;

    /// <summary>
    /// Số điện thoại
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Phòng ban / Bộ phận
    /// </summary>
    public string Department { get; set; } = string.Empty;

    /// <summary>
    /// Chức vụ
    /// </summary>
    public string Position { get; set; } = string.Empty;

    /// <summary>
    /// Địa chỉ
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Ngày bắt đầu làm việc
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Ngày kết thúc (nếu có)
    /// </summary>
    public DateTime? EndDate { get; set; }

    // Validation Methods
    public bool IsValidEmail() => Email.Contains("@") && Email.Length > 5;

    public bool IsCurrentlyActive() => IsActive && !IsDeleted && (EndDate == null || EndDate > DateTime.UtcNow);
}
