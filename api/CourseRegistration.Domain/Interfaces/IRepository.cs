using System.Linq.Expressions;

namespace CourseRegistration.Domain.Interfaces;

/// <summary>
/// Generic repository interface for basic CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets all entities asynchronously
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Gets entities with pagination asynchronously
    /// </summary>
    Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize);

    /// <summary>
    /// Gets an entity by ID asynchronously
    /// </summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Finds entities based on a predicate asynchronously
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Gets the first entity that matches the predicate asynchronously
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Adds a new entity asynchronously
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Removes an entity
    /// </summary>
    void Remove(T entity);

    /// <summary>
    /// Counts entities that match the predicate asynchronously
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

    /// <summary>
    /// Checks if any entity matches the predicate asynchronously
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}