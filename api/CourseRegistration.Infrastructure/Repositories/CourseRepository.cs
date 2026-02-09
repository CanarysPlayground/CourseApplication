using Microsoft.EntityFrameworkCore;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Interfaces;
using CourseRegistration.Infrastructure.Data;

namespace CourseRegistration.Infrastructure.Repositories;

/// <summary>
/// Course repository implementation with specific operations
/// </summary>
public class CourseRepository : Repository<Course>, ICourseRepository
{
    /// <summary>
    /// Initializes a new instance of the CourseRepository
    /// </summary>
    /// <param name="context">Database context</param>
    public CourseRepository(CourseRegistrationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets a course with its registrations asynchronously
    /// </summary>
    public async Task<Course?> GetWithRegistrationsAsync(Guid courseId)
    {
        return await _dbSet
            .Include(course => course.Registrations)
                .ThenInclude(registration => registration.Student)
            .Where(course => course.IsActive)
            .FirstOrDefaultAsync(course => course.CourseId == courseId);
    }

    /// <summary>
    /// Searches courses by name, instructor, or other criteria asynchronously
    /// </summary>
    public async Task<IEnumerable<Course>> SearchCoursesAsync(string? searchTerm, string? instructor)
    {
        var query = _dbSet.Where(course => course.IsActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(course => 
                course.CourseName.ToLower().Contains(lowerSearchTerm) ||
                (course.Description != null && course.Description.ToLower().Contains(lowerSearchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(instructor))
        {
            var lowerInstructor = instructor.ToLower();
            query = query.Where(course => course.InstructorName.ToLower().Contains(lowerInstructor));
        }

        return await query
            .OrderBy(course => course.CourseName)
            .ToListAsync();
    }

    /// <summary>
    /// Gets active courses asynchronously
    /// </summary>
    public async Task<IEnumerable<Course>> GetActiveCoursesAsync()
    {
        return await _dbSet
            .Where(course => course.IsActive)
            .OrderBy(course => course.CourseName)
            .ToListAsync();
    }

    /// <summary>
    /// Gets courses available for registration (active and not full) asynchronously
    /// </summary>
    public async Task<IEnumerable<Course>> GetAvailableCoursesAsync()
    {
        var currentDate = DateTime.UtcNow;
        
        return await _dbSet
            .Include(course => course.Registrations)
            .Where(course => course.IsActive && course.StartDate > currentDate)
            .OrderBy(course => course.StartDate)
            .ThenBy(course => course.CourseName)
            .ToListAsync();
    }

    /// <summary>
    /// Gets courses by instructor name asynchronously
    /// </summary>
    public async Task<IEnumerable<Course>> GetCoursesByInstructorAsync(string instructorName)
    {
        if (string.IsNullOrWhiteSpace(instructorName))
            return Enumerable.Empty<Course>();

        var lowerInstructorName = instructorName.ToLower();
        return await _dbSet
            .Where(course => course.IsActive && course.InstructorName.ToLower().Contains(lowerInstructorName))
            .OrderBy(course => course.CourseName)
            .ToListAsync();
    }

    /// <summary>
    /// Override GetPagedAsync to only return active courses
    /// </summary>
    public override async Task<IEnumerable<Course>> GetPagedAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        return await _dbSet
            .Where(course => course.IsActive)
            .OrderBy(course => course.CourseName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Override GetByIdAsync to only return active courses
    /// </summary>
    public override async Task<Course?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Where(course => course.IsActive)
            .FirstOrDefaultAsync(course => course.CourseId == id);
    }

    /// <summary>
    /// Soft delete implementation - mark course as inactive
    /// </summary>
    public override void Remove(Course entity)
    {
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        Update(entity);
    }
}