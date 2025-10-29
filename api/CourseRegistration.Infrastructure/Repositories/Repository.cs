using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using CourseRegistration.Domain.Interfaces;
using CourseRegistration.Infrastructure.Data;

namespace CourseRegistration.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation for basic CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly CourseRegistrationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    /// <summary>
    /// Initializes a new instance of the Repository
    /// </summary>
    /// <param name="context">Database context</param>
    public Repository(CourseRegistrationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
    }

    /// <summary>
    /// Gets all entities asynchronously
    /// </summary>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>
    /// Gets entities with pagination asynchronously
    /// </summary>
    public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        return await _dbSet
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Gets an entity by ID asynchronously
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// Finds entities based on a predicate asynchronously
    /// </summary>
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    /// <summary>
    /// Gets the first entity that matches the predicate asynchronously
    /// </summary>
    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    /// <summary>
    /// Adds a new entity asynchronously
    /// </summary>
    public virtual async Task<T> AddAsync(T entity)
    {
        var result = await _dbSet.AddAsync(entity);
        return result.Entity;
    }

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    /// <summary>
    /// Removes an entity
    /// </summary>
    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    /// <summary>
    /// Counts entities that match the predicate asynchronously
    /// </summary>
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate == null)
            return await _dbSet.CountAsync();
        
        return await _dbSet.CountAsync(predicate);
    }

    /// <summary>
    /// Checks if any entity matches the predicate asynchronously
    /// </summary>
    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}