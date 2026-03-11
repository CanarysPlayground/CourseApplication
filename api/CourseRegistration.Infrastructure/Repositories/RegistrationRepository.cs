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
            .Include(registration => registration.Course)
            .Include(registration => registration.Student)
            .Where(registration => registration.StudentId == studentId)
            .OrderByDescending(registration => registration.RegistrationDate)
            .ToListAsync();
    }

    /// <summary>
    /// Gets registrations for a specific course asynchronously
    /// </summary>
    public async Task<IEnumerable<Registration>> GetByCourseIdAsync(Guid courseId)
    {
        return await _dbSet
            .Include(registration => registration.Student)
            .Include(registration => registration.Course)
            .Where(registration => registration.CourseId == courseId)
            .OrderBy(registration => registration.Student.LastName)
            .ThenBy(registration => registration.Student.FirstName)
            .ToListAsync();
    }

    /// <summary>
    /// Gets registrations by status asynchronously
    /// </summary>
    public async Task<IEnumerable<Registration>> GetByStatusAsync(RegistrationStatus status)
    {
        return await _dbSet
            .Include(registration => registration.Student)
            .Include(registration => registration.Course)
            .Where(registration => registration.Status == status)
            .OrderByDescending(registration => registration.RegistrationDate)
            .ToListAsync();
    }

    /// <summary>
    /// Checks if a student is already registered for a course asynchronously
    /// </summary>
    public async Task<bool> IsStudentRegisteredForCourseAsync(Guid studentId, Guid courseId)
    {
        return await _dbSet
            .AnyAsync(registration => registration.StudentId == studentId && 
                          registration.CourseId == courseId && 
                          registration.Status != RegistrationStatus.Cancelled);
    }

    /// <summary>
    /// Gets registration with student and course details asynchronously
    /// </summary>
    public async Task<Registration?> GetWithDetailsAsync(Guid registrationId)
    {
        return await _dbSet
            .Include(registration => registration.Student)
            .Include(registration => registration.Course)
            .FirstOrDefaultAsync(registration => registration.RegistrationId == registrationId);
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
            .Include(registration => registration.Student)
            .Include(registration => registration.Course)
            .AsQueryable();

        if (studentId.HasValue)
        {
            query = query.Where(registration => registration.StudentId == studentId.Value);
        }

        if (courseId.HasValue)
        {
            query = query.Where(registration => registration.CourseId == courseId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(registration => registration.Status == status.Value);
        }

        return await query
            .OrderByDescending(registration => registration.RegistrationDate)
            .ToListAsync();
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
            .Include(registration => registration.Student)
            .Include(registration => registration.Course)
            .OrderByDescending(registration => registration.RegistrationDate)
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
            .Include(registration => registration.Student)
            .Include(registration => registration.Course)
            .FirstOrDefaultAsync(registration => registration.RegistrationId == id);
    }
}