namespace QLNhanVien.src.Models.Entities;

/// <summary>
/// Entity User - tài khoản đăng nhập hệ thống
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Tên đăng nhập (unique)
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Mật khẩu đã hash
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Họ tên hiển thị
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Số điện thoại
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Vai trò tài khoản (admin/employee)
    /// </summary>
    public string Status { get; set; } = UserStatuses.Admin;
}

public static class UserStatuses
{
    public const string Admin = "admin";
    public const string Employee = "employee";
}
