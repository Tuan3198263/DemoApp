using Microsoft.EntityFrameworkCore;
using QLNhanVien.src.Common.Contexts;
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
    private readonly ICurrentUserContext? _currentUserContext;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserContext currentUserContext)
        : base(options)
    {
        _currentUserContext = currentUserContext;
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Với nhiều bảng, mỗi entity nên có một class cấu hình riêng để dễ mở rộng.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditFields();

        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditFields();

        return base.SaveChanges();
    }

    private void ApplyAuditFields()
    {
        var now = DateTime.UtcNow;
        var currentUserId = _currentUserContext?.CurrentUserId;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }

            if (currentUserId.HasValue && currentUserId.Value > 0 &&
                (entry.State == EntityState.Added || entry.State == EntityState.Modified))
            {
                entry.Entity.UpdatedBy = currentUserId.Value;
            }
        }
    }
}
