using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QLNhanVien.src.Models.Entities;

namespace QLNhanVien.src.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("users", schema: "public");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("id");

        entity.Property(e => e.UserName)
            .HasColumnName("user_name")
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(e => e.Password)
            .HasColumnName("password")
            .IsRequired()
            .HasMaxLength(500);

        entity.Property(e => e.FullName)
            .HasColumnName("full_name")
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(e => e.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20);

        entity.Property(e => e.Email)
            .HasColumnName("email")
            .HasMaxLength(255);

        entity.Property(e => e.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue(UserStatuses.Admin);

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

        entity.HasIndex(e => e.UserName)
            .IsUnique()
            .HasDatabaseName("idx_users_user_name_unique");

        entity.HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("idx_users_email_unique");

        entity.HasIndex(e => e.Status)
            .HasDatabaseName("idx_users_status");

        entity.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("idx_users_created_at");
    }
}
