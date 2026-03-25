using CourseRegistration.Application.DTOs;

namespace CourseRegistration.Application.Interfaces;

/// <summary>
/// Service interface for instructor rating operations
/// </summary>
public interface IInstructorRatingService
{
    /// <summary>
    /// Creates a new instructor rating
    /// </summary>
    Task<InstructorRatingDto> CreateRatingAsync(CreateInstructorRatingDto createRatingDto);

    /// <summary>
    /// Gets a rating by ID
    /// </summary>
    Task<InstructorRatingDto?> GetRatingByIdAsync(Guid ratingId);

    /// <summary>
    /// Gets all ratings for a specific course
    /// </summary>
    Task<IEnumerable<InstructorRatingDto>> GetRatingsByCourseIdAsync(Guid courseId);

    /// <summary>
    /// Gets all ratings submitted by a specific student
    /// </summary>
    Task<IEnumerable<InstructorRatingDto>> GetRatingsByStudentIdAsync(Guid studentId);

    /// <summary>
    /// Gets all ratings for a specific instructor
    /// </summary>
    Task<IEnumerable<InstructorRatingDto>> GetRatingsByInstructorNameAsync(string instructorName);

    /// <summary>
    /// Gets rating statistics for an instructor
    /// </summary>
    Task<InstructorRatingStatsDto?> GetInstructorStatsAsync(string instructorName);

    /// <summary>
    /// Updates an existing rating
    /// </summary>
    Task<InstructorRatingDto?> UpdateRatingAsync(Guid ratingId, UpdateInstructorRatingDto updateRatingDto);

    /// <summary>
    /// Deletes a rating
    /// </summary>
    Task<bool> DeleteRatingAsync(Guid ratingId);

    /// <summary>
    /// Gets a rating by student and course
    /// </summary>
    Task<InstructorRatingDto?> GetRatingByStudentAndCourseAsync(Guid studentId, Guid courseId);
}
