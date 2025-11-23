namespace CourseRegistration.Domain.Enums;

/// <summary>
/// Represents the type of notification preference for waitlist
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Email notification
    /// </summary>
    Email = 0,

    /// <summary>
    /// In-app notification
    /// </summary>
    InApp = 1,

    /// <summary>
    /// Both email and in-app notification
    /// </summary>
    Both = 2
}
