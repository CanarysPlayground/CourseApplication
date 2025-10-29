namespace CourseRegistration.Domain.Enums;

/// <summary>
/// Represents the status of a course registration
/// </summary>
public enum RegistrationStatus
{
    /// <summary>
    /// Registration is pending approval
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Registration has been confirmed
    /// </summary>
    Confirmed = 1,

    /// <summary>
    /// Registration has been cancelled
    /// </summary>
    Cancelled = 2,

    /// <summary>
    /// Course has been completed
    /// </summary>
    Completed = 3
}