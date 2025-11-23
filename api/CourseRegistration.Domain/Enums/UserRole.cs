namespace CourseRegistration.Domain.Enums;

/// <summary>
/// Represents the role of a user in the system
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Regular student user
    /// </summary>
    Student = 0,

    /// <summary>
    /// Instructor with course management capabilities
    /// </summary>
    Instructor = 1,

    /// <summary>
    /// Administrator with full system access
    /// </summary>
    Admin = 2,

    /// <summary>
    /// Super administrator with elevated privileges
    /// </summary>
    SuperAdmin = 3
}