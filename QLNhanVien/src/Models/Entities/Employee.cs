namespace QLNhanVien.src.Models.Entities;

/// <summary>
/// Entity Employee - Nhân viên
/// </summary>
public class Employee : BaseEntity
{
    /// <summary>
    /// Mã nhân viên (unique)
    /// </summary>
    public string MaNhanVien { get; set; } = string.Empty;

    /// <summary>
    /// Tên nhân viên
    /// </summary>
    public string TenNhanVien { get; set; } = string.Empty;

    /// <summary>
    /// Ngày sinh
    /// </summary>
    public DateOnly NgaySinh { get; set; }

    /// <summary>
    /// Giới tính (Nam/Nữ/Khác)
    /// </summary>
    public string GioiTinh { get; set; } = GioiTinhConstants.Nam;

    /// <summary>
    /// Bộ phận/Phòng ban
    /// </summary>
    public string BoPhan { get; set; } = string.Empty;

    /// <summary>
    /// Mức lương
    /// </summary>
    public decimal MucLuong { get; set; }
}

public static class GioiTinhConstants
{
    public const string Nam = "Nam";
    public const string Nu = "Nữ";
    public const string Khac = "Khác";
}
