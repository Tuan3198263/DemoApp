using Microsoft.EntityFrameworkCore;
using QLNhanVien.src.Models.Entities;

namespace QLNhanVien.src.Data.Repositories;

/// <summary>
/// Generic Repository Interface
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    // Query
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default);
    Task<List<T>> FindAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Func<T, bool>? predicate = null, CancellationToken cancellationToken = default);

    // Modification
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    // Pagination
    Task<(List<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Func<IQueryable<T>, IQueryable<T>>? filter = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Generic Repository Implementation
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;
    private readonly ILogger<Repository<T>> _logger;

    public Repository(AppDbContext context, ILogger<Repository<T>> logger)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _logger = logger;
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && e.DeletedAt == null, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving entity with id {Id}", id);
            throw;
        }
    }

    public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet.Where(e => e.DeletedAt == null).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all entities");
            throw;
        }
    }

    public async Task<T?> FirstOrDefaultAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await Task.FromResult(
                _dbSet.Where(e => e.DeletedAt == null).FirstOrDefault(predicate)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving first entity");
            throw;
        }
    }

    public async Task<List<T>> FindAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await Task.FromResult(
                _dbSet.Where(e => e.DeletedAt == null).Where(predicate).ToList()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding entities");
            throw;
        }
    }

    public async Task<int> CountAsync(Func<T, bool>? predicate = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _dbSet.Where(e => e.DeletedAt == null);
            if (predicate != null)
                query = query.Where(predicate).AsQueryable();

            return await Task.FromResult(query.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting entities");
            throw;
        }
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Entity added with id {Id}", entity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding entity");
            throw;
        }
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            var entitiesList = entities.ToList();
            foreach (var entity in entitiesList)
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
            }
            await _dbSet.AddRangeAsync(entitiesList, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Added {Count} entities", entitiesList.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding multiple entities");
            throw;
        }
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Entity with id {Id} updated", entity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity with id {Id}", entity.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("Entity with id {Id} not found", id);
                return;
            }

            // Soft delete
            entity.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(entity, cancellationToken);
            _logger.LogInformation("Entity with id {Id} soft deleted", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity with id {Id}", id);
            throw;
        }
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            // Soft delete
            entity.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(entity, cancellationToken);
            _logger.LogInformation("Entity with id {Id} soft deleted", entity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity");
            throw;
        }
    }

    public async Task<(List<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Func<IQueryable<T>, IQueryable<T>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _dbSet.Where(e => e.DeletedAt == null).AsQueryable();

            if (filter != null)
                query = filter(query);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged entities");
            throw;
        }
    }
}
