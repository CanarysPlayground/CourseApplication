using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Application.Services;

/// <summary>
/// Service for handling user authorization and access control
/// </summary>
public class AuthorizationService
{
    /// <summary>
    /// Check if a user has admin access
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <returns>True if user has admin access, false otherwise</returns>
    public bool HasAdminAccess(User user)
    {
        if (user == null)
            return false;

        if (!user.IsActive)
            return false;

        return user.Role == UserRole.Admin || user.Role == UserRole.SuperAdmin;
    }

    /// <summary>
    /// Check if a user has admin access by user ID
    /// </summary>
    /// <param name="userId">The ID of the user to check</param>
    /// <param name="userRepository">Repository to fetch user data</param>
    /// <returns>True if user has admin access, false otherwise</returns>
    public async Task<bool> HasAdminAccessAsync(Guid userId, IUserRepository userRepository)
    {
        var user = await userRepository.GetByIdAsync(userId);
        return HasAdminAccess(user);
    }

    /// <summary>
    /// Check if a user has instructor access
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <returns>True if user has instructor access, false otherwise</returns>
    public bool HasInstructorAccess(User user)
    {
        if (user == null)
            return false;

        if (!user.IsActive)
            return false;

        return user.Role == UserRole.Instructor || HasAdminAccess(user);
    }

    /// <summary>
    /// Check if a user has instructor access by user ID
    /// </summary>
    /// <param name="userId">The ID of the user to check</param>
    /// <param name="userRepository">Repository to fetch user data</param>
    /// <returns>True if user has instructor access, false otherwise</returns>
    public async Task<bool> HasInstructorAccessAsync(Guid userId, IUserRepository userRepository)
    {
        var user = await userRepository.GetByIdAsync(userId);
        return HasInstructorAccess(user);
    }

    /// <summary>
    /// Verify that a user has admin access, throws exception if not
    /// </summary>
    /// <param name="user">The user to verify</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when user lacks admin access</exception>
    public void RequireAdminAccess(User user)
    {
        if (!HasAdminAccess(user))
        {
            throw new UnauthorizedAccessException("Admin access required for this operation");
        }
    }

    /// <summary>
    /// Verify that a user has instructor access, throws exception if not
    /// </summary>
    /// <param name="user">The user to verify</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when user lacks instructor access</exception>
    public void RequireInstructorAccess(User user)
    {
        if (!HasInstructorAccess(user))
        {
            throw new UnauthorizedAccessException("Instructor access required for this operation");
        }
    }

    /// <summary>
    /// Check if a user can perform a specific action
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <param name="action">The action to perform</param>
    /// <param name="targetUserId">The ID of the target user (for user-specific operations)</param>
    /// <returns>True if user can perform the action, false otherwise</returns>
    public bool CanPerformAction(User user, string action, Guid? targetUserId = null)
    {
        if (user == null || !user.IsActive)
            return false;

        return action.ToLower() switch
        {
            "create_course" => HasInstructorAccess(user),
            "delete_course" => HasAdminAccess(user),
            "manage_users" => HasAdminAccess(user),
            "view_all_registrations" => HasInstructorAccess(user),
            "modify_grades" => HasInstructorAccess(user),
            "generate_certificates" => HasInstructorAccess(user),
            "view_own_data" => targetUserId == null || targetUserId == user.UserId || HasInstructorAccess(user),
            "modify_own_data" => targetUserId == null || targetUserId == user.UserId || HasAdminAccess(user),
            _ => HasAdminAccess(user) // Default: only admins can perform unknown actions
        };
    }
}

/// <summary>
/// Repository interface for User entity operations
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}