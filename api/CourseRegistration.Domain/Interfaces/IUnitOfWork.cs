namespace CourseRegistration.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern interface to manage transactions across repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Student repository
    /// </summary>
    IStudentRepository Students { get; }

    /// <summary>
    /// Course repository
    /// </summary>
    ICourseRepository Courses { get; }

    /// <summary>
    /// Registration repository
    /// </summary>
    IRegistrationRepository Registrations { get; }

    /// <summary>
    /// Saves all changes made in this unit of work asynchronously
    /// </summary>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Begins a new database transaction asynchronously
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits the current transaction asynchronously
    /// </summary>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rolls back the current transaction asynchronously
    /// </summary>
    Task RollbackTransactionAsync();
}