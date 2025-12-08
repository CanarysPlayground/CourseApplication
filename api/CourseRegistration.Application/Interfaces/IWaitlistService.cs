using CourseRegistration.Application.DTOs;

namespace CourseRegistration.Application.Interfaces;

/// <summary>
/// Service interface for waitlist operations
/// </summary>
public interface IWaitlistService
{
    /// <summary>
    /// Adds a student to a course waitlist
    /// </summary>
    Task<WaitlistEntryDto> JoinWaitlistAsync(CreateWaitlistEntryDto createWaitlistEntryDto);

    /// <summary>
    /// Removes a student from a course waitlist
    /// </summary>
    Task<bool> LeaveWaitlistAsync(Guid waitlistEntryId);

    /// <summary>
    /// Gets active waitlist entries for a course
    /// </summary>
    Task<IEnumerable<WaitlistEntryDto>> GetCourseWaitlistAsync(Guid courseId);

    /// <summary>
    /// Gets a student's active waitlist entries
    /// </summary>
    Task<IEnumerable<WaitlistEntryDto>> GetStudentWaitlistsAsync(Guid studentId);

    /// <summary>
    /// Gets a specific waitlist entry by ID
    /// </summary>
    Task<WaitlistEntryDto?> GetWaitlistEntryAsync(Guid waitlistEntryId);

    /// <summary>
    /// Updates a waitlist entry (admin function for reordering or updating preferences)
    /// </summary>
    Task<WaitlistEntryDto?> UpdateWaitlistEntryAsync(Guid waitlistEntryId, UpdateWaitlistEntryDto updateDto);

    /// <summary>
    /// Notifies the next student on the waitlist when a spot becomes available
    /// </summary>
    Task NotifyNextStudentAsync(Guid courseId);

    /// <summary>
    /// Clears the entire waitlist for a course (admin function)
    /// </summary>
    Task<bool> ClearWaitlistAsync(Guid courseId);

    /// <summary>
    /// Reorders waitlist entries (admin function)
    /// </summary>
    Task<bool> ReorderWaitlistAsync(Guid courseId, Dictionary<Guid, int> newPositions);

    /// <summary>
    /// Checks if a student is on the waitlist for a course
    /// </summary>
    Task<bool> IsStudentOnWaitlistAsync(Guid studentId, Guid courseId);
}
