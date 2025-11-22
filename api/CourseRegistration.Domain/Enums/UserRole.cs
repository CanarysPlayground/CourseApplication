namespace CourseRegistration.Domain.Enums;

/// <summary>
/// Represents the role of a user in the system
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Regular user with limited permissions
    /// </summary>
    User = 0,

    /// <summary>
    /// Administrator with full permissions
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Instructor with course management permissions
    /// </summary>
    Instructor = 2
}
