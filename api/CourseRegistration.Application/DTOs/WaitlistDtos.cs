using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Application.DTOs;

/// <summary>
/// Data transfer object for creating a waitlist entry
/// </summary>
public class CreateWaitlistEntryDto
{
    /// <summary>
    /// Student ID
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// Course ID
    /// </summary>
    public Guid CourseId { get; set; }

    /// <summary>
    /// Notification preference
    /// </summary>
    public NotificationType NotificationPreference { get; set; } = NotificationType.Email;
}

/// <summary>
/// Data transfer object for waitlist entry response
/// </summary>
public class WaitlistEntryDto
{
    /// <summary>
    /// Waitlist entry ID
    /// </summary>
    public Guid WaitlistEntryId { get; set; }

    /// <summary>
    /// Student ID
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// Student's full name
    /// </summary>
    public string StudentName { get; set; } = string.Empty;

    /// <summary>
    /// Student's email
    /// </summary>
    public string StudentEmail { get; set; } = string.Empty;

    /// <summary>
    /// Course ID
    /// </summary>
    public Guid CourseId { get; set; }

    /// <summary>
    /// Course name
    /// </summary>
    public string CourseName { get; set; } = string.Empty;

    /// <summary>
    /// Position in the waitlist
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Date and time when joined the waitlist
    /// </summary>
    public DateTime JoinedAt { get; set; }

    /// <summary>
    /// Notification preference
    /// </summary>
    public NotificationType NotificationPreference { get; set; }

    /// <summary>
    /// Indicates if still active on waitlist
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Date and time when notified
    /// </summary>
    public DateTime? NotifiedAt { get; set; }

    /// <summary>
    /// Notes about the waitlist entry
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Data transfer object for updating waitlist entry
/// </summary>
public class UpdateWaitlistEntryDto
{
    /// <summary>
    /// New position in the waitlist (for admin reordering)
    /// </summary>
    public int? Position { get; set; }

    /// <summary>
    /// Notification preference
    /// </summary>
    public NotificationType? NotificationPreference { get; set; }

    /// <summary>
    /// Notes about the waitlist entry
    /// </summary>
    public string? Notes { get; set; }
}
