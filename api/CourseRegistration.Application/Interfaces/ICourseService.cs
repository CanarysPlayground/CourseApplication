using CourseRegistration.Application.DTOs;

namespace CourseRegistration.Application.Interfaces;

/// <summary>
/// Service interface for course operations
/// </summary>
public interface ICourseService
{
    /// <summary>
    /// Gets all courses with pagination and filtering
    /// </summary>
    Task<PagedResponseDto<CourseDto>> GetCoursesAsync(int page = 1, int pageSize = 10, string? searchTerm = null, string? instructor = null);

    /// <summary>
    /// Gets a course by ID
    /// </summary>
    Task<CourseDto?> GetCourseByIdAsync(Guid id);

    /// <summary>
    /// Creates a new course
    /// </summary>
    Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto);

    /// <summary>
    /// Updates an existing course
    /// </summary>
    Task<CourseDto?> UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto);

    /// <summary>
    /// Deletes a course (soft delete)
    /// </summary>
    Task<bool> DeleteCourseAsync(Guid id);

    /// <summary>
    /// Searches courses by name, instructor, or description
    /// </summary>
    Task<IEnumerable<CourseDto>> SearchCoursesAsync(string? searchTerm, string? instructor);

    /// <summary>
    /// Gets available courses for registration
    /// </summary>
    Task<IEnumerable<CourseDto>> GetAvailableCoursesAsync();

    /// <summary>
    /// Gets courses by instructor
    /// </summary>
    Task<IEnumerable<CourseDto>> GetCoursesByInstructorAsync(string instructorName);

    /// <summary>
    /// Gets course registrations
    /// </summary>
    Task<IEnumerable<RegistrationDto>> GetCourseRegistrationsAsync(Guid courseId);
}