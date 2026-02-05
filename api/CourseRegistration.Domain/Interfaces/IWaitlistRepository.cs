using CourseRegistration.Domain.Entities;

namespace CourseRegistration.Domain.Interfaces;

/// <summary>
/// Repository interface for WaitlistEntry operations
/// </summary>
public interface IWaitlistRepository : IRepository<WaitlistEntry>
{
    /// <summary>
    /// Gets active waitlist entries for a specific course, ordered by position
    /// </summary>
    Task<IEnumerable<WaitlistEntry>> GetActiveWaitlistForCourseAsync(Guid courseId);

    /// <summary>
    /// Gets a student's active waitlist entry for a specific course
    /// </summary>
    Task<WaitlistEntry?> GetActiveWaitlistEntryAsync(Guid studentId, Guid courseId);

    /// <summary>
    /// Gets all active waitlist entries for a student
    /// </summary>
    Task<IEnumerable<WaitlistEntry>> GetStudentActiveWaitlistEntriesAsync(Guid studentId);

    /// <summary>
    /// Checks if a student is on the active waitlist for a course
    /// </summary>
    Task<bool> IsStudentOnWaitlistAsync(Guid studentId, Guid courseId);

    /// <summary>
    /// Gets the next available position for a course waitlist
    /// </summary>
    Task<int> GetNextPositionAsync(Guid courseId);

    /// <summary>
    /// Gets waitlist entry with student and course details
    /// </summary>
    Task<WaitlistEntry?> GetWithDetailsAsync(Guid waitlistEntryId);

    /// <summary>
    /// Reorders waitlist positions after a student is removed
    /// </summary>
    Task ReorderWaitlistAsync(Guid courseId, int removedPosition);
}
