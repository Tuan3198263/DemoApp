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
    private readonly ILogger<AppDbContext> _logger;

    public AppDbContext(DbContextOptions<AppDbContext> options, ILogger<AppDbContext> logger)
        : base(options)
    {
        _logger = logger;
    }

    // DbSets
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users", schema: "public");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.FullName)
                .HasColumnName("full_name")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.EmployeeCode)
                .HasColumnName("employee_code")
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.PhoneNumber)
                .HasColumnName("phone_number")
                .HasMaxLength(20);

            entity.Property(e => e.Department)
                .HasColumnName("department")
                .HasMaxLength(100);

            entity.Property(e => e.Position)
                .HasColumnName("position")
                .HasMaxLength(100);

            entity.Property(e => e.Address)
                .HasColumnName("address")
                .HasMaxLength(500);

            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            entity.Property(e => e.StartDate)
                .HasColumnName("start_date");

            entity.Property(e => e.EndDate)
                .HasColumnName("end_date");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValue(DateTime.UtcNow);

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValue(DateTime.UtcNow);

            entity.Property(e => e.DeletedAt)
                .HasColumnName("deleted_at");

            // Indexes
            entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("idx_users_email_unique");
            entity.HasIndex(e => e.EmployeeCode).IsUnique().HasDatabaseName("idx_users_employee_code_unique");
            entity.HasIndex(e => e.IsActive).HasDatabaseName("idx_users_is_active");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("idx_users_created_at");
        });

        // Seed default data (optional)
        // modelBuilder.Entity<User>().HasData(
        //     new User { Id = 1, FullName = "Admin", Email = "admin@example.com", ... }
        // );
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

        try
        {
            _logger.LogDebug("Saving changes to database");
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency conflict: {Message}", ex.Message);
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update error: {Message}", ex.Message);
            throw;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Database operation cancelled");
            throw;
        }
    }
}
