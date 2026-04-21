namespace QLNhanVien.src.Models.Entities;

/// Entity User - tài khoản đăng nhập hệ thống
public class User : BaseEntity
{

    /// Tên đăng nhập (unique)
    public string UserName { get; set; } = string.Empty;

    /// Mật khẩu đã hash
    public string Password { get; set; } = string.Empty;

    /// Họ tên hiển thị
    public string FullName { get; set; } = string.Empty;

    /// Số điện thoại
    public string Phone { get; set; } = string.Empty;

    /// Email
    public string Email { get; set; } = string.Empty;

    /// Vai trò tài khoản (admin/employee)
    public string Status { get; set; } = UserStatuses.Admin;
}

public static class UserStatuses
{
    public const string Admin = "admin";
    public const string Employee = "employee";
}
