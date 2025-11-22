using CourseRegistration.Domain.Entities;

namespace CourseRegistration.Application.Interfaces;

/// <summary>
/// Interface for admin access validation service
/// </summary>
public interface IAdminAccessService
{
    /// <summary>
    /// Checks if a user has admin access
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <returns>True if the user has admin access, false otherwise</returns>
    bool HasAdminAccess(User? user);

    /// <summary>
    /// Checks if a user has admin access by user ID
    /// </summary>
    /// <param name="userId">The user ID to check</param>
    /// <returns>True if the user has admin access, false otherwise</returns>
    Task<bool> HasAdminAccessAsync(Guid userId);
}
