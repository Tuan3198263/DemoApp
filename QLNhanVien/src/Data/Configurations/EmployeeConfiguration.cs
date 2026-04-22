using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QLNhanVien.src.Models.Entities;

namespace QLNhanVien.src.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> entity)
    {
        entity.ToTable("employees", schema: "public");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("id");

        entity.Property(e => e.MaNhanVien)
            .HasColumnName("ma_nhan_vien")
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.TenNhanVien)
            .HasColumnName("ten_nhan_vien")
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.NgaySinh)
            .HasColumnName("ngay_sinh")
            .HasColumnType("date")
            .IsRequired();

        entity.Property(e => e.GioiTinh)
            .HasColumnName("gioi_tinh")
            .HasMaxLength(10)
            .HasDefaultValue(GioiTinhConstants.Nam);

        entity.Property(e => e.BoPhan)
            .HasColumnName("bo_phan")
            .HasMaxLength(255);

        entity.Property(e => e.MucLuong)
            .HasColumnName("muc_luong")
            .HasPrecision(18, 2);

        entity.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now() at time zone 'utc'");

        entity.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("now() at time zone 'utc'");

        entity.Property(e => e.UpdatedBy)
            .HasColumnName("updated_by");

        entity.Property(e => e.DeletedAt)
            .HasColumnName("deleted_at");

        entity.HasIndex(e => e.MaNhanVien)
            .IsUnique()
            .HasDatabaseName("idx_employees_ma_nhan_vien_unique");

        entity.HasIndex(e => e.GioiTinh)
            .HasDatabaseName("idx_employees_gioi_tinh");

        entity.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("idx_employees_created_at");

        entity.HasIndex(e => e.UpdatedBy)
            .HasDatabaseName("idx_employees_updated_by");
    }
}
