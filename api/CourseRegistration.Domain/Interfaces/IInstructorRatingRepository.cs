using CourseRegistration.Domain.Entities;

namespace CourseRegistration.Domain.Interfaces;

/// <summary>
/// InstructorRating repository interface with specific operations
/// </summary>
public interface IInstructorRatingRepository : IRepository<InstructorRating>
{
    /// <summary>
    /// Gets all ratings for a specific course
    /// </summary>
    Task<IEnumerable<InstructorRating>> GetByCourseIdAsync(Guid courseId);

    /// <summary>
    /// Gets all ratings submitted by a specific student
    /// </summary>
    Task<IEnumerable<InstructorRating>> GetByStudentIdAsync(Guid studentId);

    /// <summary>
    /// Gets all ratings for a specific instructor across all courses
    /// </summary>
    Task<IEnumerable<InstructorRating>> GetByInstructorNameAsync(string instructorName);

    /// <summary>
    /// Gets a specific rating by student and course
    /// </summary>
    Task<InstructorRating?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId);

    /// <summary>
    /// Checks if a student has already rated a course
    /// </summary>
    Task<bool> HasStudentRatedCourseAsync(Guid studentId, Guid courseId);

    /// <summary>
    /// Gets average rating for an instructor
    /// </summary>
    Task<double?> GetAverageRatingForInstructorAsync(string instructorName);

    /// <summary>
    /// Gets rating with student and course details
    /// </summary>
    Task<InstructorRating?> GetWithDetailsAsync(Guid ratingId);
}
