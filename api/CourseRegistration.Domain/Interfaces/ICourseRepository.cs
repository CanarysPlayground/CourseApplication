using CourseRegistration.Domain.Entities;

namespace CourseRegistration.Domain.Interfaces;

/// <summary>
/// Course repository interface with specific operations
/// </summary>
public interface ICourseRepository : IRepository<Course>
{
    /// <summary>
    /// Gets a course with its registrations asynchronously
    /// </summary>
    Task<Course?> GetWithRegistrationsAsync(Guid courseId);

    /// <summary>
    /// Searches courses by name, instructor, or other criteria asynchronously
    /// </summary>
    Task<IEnumerable<Course>> SearchCoursesAsync(string? searchTerm, string? instructor);

    /// <summary>
    /// Gets active courses asynchronously
    /// </summary>
    Task<IEnumerable<Course>> GetActiveCoursesAsync();

    /// <summary>
    /// Gets courses available for registration (active and not full) asynchronously
    /// </summary>
    Task<IEnumerable<Course>> GetAvailableCoursesAsync();

    /// <summary>
    /// Gets courses by instructor name asynchronously
    /// </summary>
    Task<IEnumerable<Course>> GetCoursesByInstructorAsync(string instructorName);
}