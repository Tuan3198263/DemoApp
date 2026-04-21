using Microsoft.EntityFrameworkCore;
using QLNhanVien.src.Models.Entities;

namespace QLNhanVien.src.Data;

/// <summary>
/// AppDbContext - Entity Framework Core context cho PostgreSQL Supabase
/// 
/// Configuration:
/// - Connection pooling: Tối ưu performance
/// - Retry logic: Xử lý network issues
/// - Logging: Debug các lỗi connection
/// - SSL Mode: Bảo mật kết nối tới Supabase
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Với nhiều bảng, mỗi entity nên có một class cấu hình riêng để dễ mở rộng.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-update UpdatedAt trước khi save
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is BaseEntity baseEntity)
            {
                baseEntity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
