using Microsoft.EntityFrameworkCore;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Enums;
using CourseRegistration.Domain.Interfaces;
using CourseRegistration.Infrastructure.Data;

namespace CourseRegistration.Infrastructure.Repositories;

/// <summary>
/// Registration repository implementation with specific operations
/// </summary>
public class RegistrationRepository : Repository<Registration>, IRegistrationRepository
{
    /// <summary>
    /// Initializes a new instance of the RegistrationRepository
    /// </summary>
    /// <param name="context">Database context</param>
    public RegistrationRepository(CourseRegistrationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets registrations for a specific student asynchronously
    /// </summary>
    public async Task<IEnumerable<Registration>> GetByStudentIdAsync(Guid studentId)
    {
        return await _dbSet
            .Include(r => r.Course)
            .Include(r => r.Student)
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.RegistrationDate)
            .ToListAsync();
    }

    /// <summary>
    /// Gets registrations for a specific course asynchronously
    /// </summary>
    public async Task<IEnumerable<Registration>> GetByCourseIdAsync(Guid courseId)
    {
        return await _dbSet
            .Include(r => r.Student)
            .Include(r => r.Course)
            .Where(r => r.CourseId == courseId)
            .OrderBy(r => r.Student.LastName)
            .ThenBy(r => r.Student.FirstName)
            .ToListAsync();
    }

    /// <summary>
    /// Gets registrations by status asynchronously
    /// </summary>
    public async Task<IEnumerable<Registration>> GetByStatusAsync(RegistrationStatus status)
    {
        return await _dbSet
            .Include(r => r.Student)
            .Include(r => r.Course)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.RegistrationDate)
            .ToListAsync();
    }

    /// <summary>
    /// Checks if a student is already registered for a course asynchronously
    /// </summary>
    public async Task<bool> IsStudentRegisteredForCourseAsync(Guid studentId, Guid courseId)
    {
        return await _dbSet
            .AnyAsync(r => r.StudentId == studentId && 
                          r.CourseId == courseId && 
                          r.Status != RegistrationStatus.Cancelled);
    }

    /// <summary>
    /// Gets registration with student and course details asynchronously
    /// </summary>
    public async Task<Registration?> GetWithDetailsAsync(Guid registrationId)
    {
        return await _dbSet
            .Include(r => r.Student)
            .Include(r => r.Course)
            .FirstOrDefaultAsync(r => r.RegistrationId == registrationId);
    }

    /// <summary>
    /// Gets registrations with filtering options asynchronously
    /// </summary>
    public async Task<IEnumerable<Registration>> GetRegistrationsWithFiltersAsync(
        Guid? studentId = null, 
        Guid? courseId = null, 
        RegistrationStatus? status = null)
    {
        var query = _dbSet
            .Include(r => r.Student)
            .Include(r => r.Course)
            .AsQueryable();

        if (studentId.HasValue)
        {
            query = query.Where(r => r.StudentId == studentId.Value);
        }

        if (courseId.HasValue)
        {
            query = query.Where(r => r.CourseId == courseId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        return await query
            .OrderByDescending(r => r.RegistrationDate)
            .ToListAsync();
    }

    /// <summary>
    /// Gets paginated registrations with filtering options asynchronously
    /// </summary>
    public async Task<IEnumerable<Registration>> GetPagedRegistrationsWithFiltersAsync(
        int page,
        int pageSize,
        Guid? studentId = null,
        Guid? courseId = null,
        RegistrationStatus? status = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = _dbSet
            .Include(r => r.Student)
            .Include(r => r.Course)
            .AsQueryable();

        if (studentId.HasValue)
        {
            query = query.Where(r => r.StudentId == studentId.Value);
        }

        if (courseId.HasValue)
        {
            query = query.Where(r => r.CourseId == courseId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        return await query
            .OrderByDescending(r => r.RegistrationDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Gets count of registrations matching filters asynchronously
    /// </summary>
    public async Task<int> CountRegistrationsWithFiltersAsync(
        Guid? studentId = null,
        Guid? courseId = null,
        RegistrationStatus? status = null)
    {
        var query = _dbSet.AsQueryable();

        if (studentId.HasValue)
        {
            query = query.Where(r => r.StudentId == studentId.Value);
        }

        if (courseId.HasValue)
        {
            query = query.Where(r => r.CourseId == courseId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        return await query.CountAsync();
    }

    /// <summary>
    /// Override GetPagedAsync to include related entities
    /// </summary>
    public override async Task<IEnumerable<Registration>> GetPagedAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        return await _dbSet
            .Include(r => r.Student)
            .Include(r => r.Course)
            .OrderByDescending(r => r.RegistrationDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Override GetByIdAsync to include related entities
    /// </summary>
    public override async Task<Registration?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(r => r.Student)
            .Include(r => r.Course)
            .FirstOrDefaultAsync(r => r.RegistrationId == id);
    }
}