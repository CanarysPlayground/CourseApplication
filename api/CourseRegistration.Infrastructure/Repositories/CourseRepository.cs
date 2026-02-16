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
    /// Escapes SQL LIKE pattern special characters
    /// Uses ^ as the escape character to avoid conflicts with % and _
    /// </summary>
    private static string EscapeLikePattern(string pattern)
    {
        return pattern.Replace("^", "^^")
                      .Replace("%", "^%")
                      .Replace("_", "^_");
    }

    /// <summary>
    /// Gets a course with its registrations asynchronously
    /// </summary>
    public async Task<Course?> GetWithRegistrationsAsync(Guid courseId)
    {
        return await _dbSet
            .Include(c => c.Registrations)
                .ThenInclude(r => r.Student)
            .Where(c => c.IsActive)
            .FirstOrDefaultAsync(c => c.CourseId == courseId);
    }

    /// <summary>
    /// Searches courses by name, instructor, or other criteria asynchronously
    /// </summary>
    public async Task<IEnumerable<Course>> SearchCoursesAsync(string? searchTerm, string? instructor)
    {
        var query = _dbSet.Where(c => c.IsActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var escapedTerm = EscapeLikePattern(searchTerm);
            var searchPattern = $"%{escapedTerm}%";
            query = query.Where(c => 
                EF.Functions.Like(c.CourseName, searchPattern, "^") ||
                (c.Description != null && EF.Functions.Like(c.Description, searchPattern, "^")));
        }

        if (!string.IsNullOrWhiteSpace(instructor))
        {
            var escapedInstructor = EscapeLikePattern(instructor);
            var instructorPattern = $"%{escapedInstructor}%";
            query = query.Where(c => EF.Functions.Like(c.InstructorName, instructorPattern, "^"));
        }

        return await query
            .OrderBy(c => c.CourseName)
            .ToListAsync();
    }

    /// <summary>
    /// Gets active courses asynchronously
    /// </summary>
    public async Task<IEnumerable<Course>> GetActiveCoursesAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.CourseName)
            .ToListAsync();
    }

    /// <summary>
    /// Gets courses available for registration (active and not full) asynchronously
    /// </summary>
    public async Task<IEnumerable<Course>> GetAvailableCoursesAsync()
    {
        var currentDate = DateTime.UtcNow;
        
        return await _dbSet
            .Include(c => c.Registrations)
            .Where(c => c.IsActive && c.StartDate > currentDate)
            .OrderBy(c => c.StartDate)
            .ThenBy(c => c.CourseName)
            .ToListAsync();
    }

    /// <summary>
    /// Gets courses by instructor name asynchronously
    /// </summary>
    public async Task<IEnumerable<Course>> GetCoursesByInstructorAsync(string instructorName)
    {
        if (string.IsNullOrWhiteSpace(instructorName))
            return Enumerable.Empty<Course>();

        var escapedInstructor = EscapeLikePattern(instructorName);
        var instructorPattern = $"%{escapedInstructor}%";
        return await _dbSet
            .Where(c => c.IsActive && EF.Functions.Like(c.InstructorName, instructorPattern, "^"))
            .OrderBy(c => c.CourseName)
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
            .Where(c => c.IsActive)
            .OrderBy(c => c.CourseName)
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
            .Where(c => c.IsActive)
            .FirstOrDefaultAsync(c => c.CourseId == id);
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