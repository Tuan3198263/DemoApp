namespace QLNhanVien.src.Models.Entities;

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Base entity với Id, CreatedAt, UpdatedAt - tất cả entities đều inherit từ đây
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Primary Key
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Thời gian tạo (UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thời gian cập nhật gần nhất (UTC)
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thời gian xóa (null nếu chưa xóa) - dùng cho soft delete
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Kiểm tra entity có bị soft delete hay không
    /// </summary>
    [NotMapped]
    public bool IsDeleted => DeletedAt.HasValue;
}
