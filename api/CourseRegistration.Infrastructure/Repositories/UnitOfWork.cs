using Microsoft.EntityFrameworkCore.Storage;
using CourseRegistration.Domain.Interfaces;
using CourseRegistration.Infrastructure.Data;
using CourseRegistration.Infrastructure.Repositories;

namespace CourseRegistration.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation to manage transactions across repositories
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly CourseRegistrationDbContext _context;
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed = false;

    // Lazy initialization of repositories
    private IStudentRepository? _students;
    private ICourseRepository? _courses;
    private IRegistrationRepository? _registrations;
    private IWaitlistRepository? _waitlists;

    /// <summary>
    /// Initializes a new instance of the UnitOfWork
    /// </summary>
    /// <param name="context">Database context</param>
    public UnitOfWork(CourseRegistrationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Student repository
    /// </summary>
    public IStudentRepository Students
    {
        get
        {
            _students ??= new StudentRepository(_context);
            return _students;
        }
    }

    /// <summary>
    /// Course repository
    /// </summary>
    public ICourseRepository Courses
    {
        get
        {
            _courses ??= new CourseRepository(_context);
            return _courses;
        }
    }

    /// <summary>
    /// Registration repository
    /// </summary>
    public IRegistrationRepository Registrations
    {
        get
        {
            _registrations ??= new RegistrationRepository(_context);
            return _registrations;
        }
    }

    /// <summary>
    /// Waitlist repository
    /// </summary>
    public IWaitlistRepository Waitlists
    {
        get
        {
            _waitlists ??= new WaitlistRepository(_context);
            return _waitlists;
        }
    }

    /// <summary>
    /// Saves all changes made in this unit of work asynchronously
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            // Log the exception if needed
            throw;
        }
    }

    /// <summary>
    /// Begins a new database transaction asynchronously
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Commits the current transaction asynchronously
    /// </summary>
    public async Task CommitTransactionAsync()
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction in progress.");
        }

        try
        {
            await SaveChangesAsync();
            await _currentTransaction.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction asynchronously
    /// </summary>
    public async Task RollbackTransactionAsync()
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction in progress.");
        }

        try
        {
            await _currentTransaction.RollbackAsync();
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    /// <summary>
    /// Disposes the unit of work and associated resources
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected dispose method
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _currentTransaction?.Dispose();
            _context?.Dispose();
            _disposed = true;
        }
    }
}