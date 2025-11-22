using CourseRegistration.Application.Interfaces;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CourseRegistration.Application.Services;

/// <summary>
/// Service for validating admin access with security best practices
/// </summary>
public class AdminAccessService : IAdminAccessService
{
    private readonly ILogger<AdminAccessService> _logger;

    public AdminAccessService(ILogger<AdminAccessService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Checks if a user has admin access with proper validation
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <returns>True if the user has admin access, false otherwise</returns>
    public bool HasAdminAccess(User? user)
    {
        // Security: Return false for null users instead of throwing
        if (user == null)
        {
            _logger.LogWarning("Admin access check failed: User is null");
            return false;
        }

        // Security: Check if user account is active
        if (!user.IsActive)
        {
            _logger.LogWarning("Admin access check failed: User {UserId} is not active", user.UserId);
            return false;
        }

        // Security: Validate user ID is not empty
        if (user.UserId == Guid.Empty)
        {
            _logger.LogWarning("Admin access check failed: User ID is empty");
            return false;
        }

        // Security: Check role with explicit comparison
        bool hasAccess = user.Role == UserRole.Admin;

        if (hasAccess)
        {
            _logger.LogInformation("Admin access granted for user {UserId}", user.UserId);
        }
        else
        {
            _logger.LogInformation("Admin access denied for user {UserId} with role {Role}", 
                user.UserId, user.Role);
        }

        return hasAccess;
    }

    /// <summary>
    /// Checks if a user has admin access by user ID (async version for future database lookup)
    /// </summary>
    /// <param name="userId">The user ID to check</param>
    /// <returns>True if the user has admin access, false otherwise</returns>
    /// <exception cref="NotImplementedException">This method requires database context and is not yet implemented</exception>
    public Task<bool> HasAdminAccessAsync(Guid userId)
    {
        // Security: Validate user ID
        if (userId == Guid.Empty)
        {
            _logger.LogWarning("Admin access check failed: Invalid user ID");
            return Task.FromResult(false);
        }

        // Note: This method requires database context to query the user
        throw new NotImplementedException("This method requires database context and is not yet implemented. Use HasAdminAccess with a User object instead.");
    }
}
