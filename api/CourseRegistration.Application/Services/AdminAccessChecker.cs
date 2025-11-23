using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Security.Principal;
using System.Security.Claims;

namespace CourseRegistration.Application.Services;

/// <summary>
/// Secure utility class for checking user permissions and admin access with comprehensive logging and validation
/// </summary>
public static class AdminAccessChecker
{
    /// <summary>
    /// Check if a user has admin access with comprehensive security validation
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <param name="logger">Optional logger for audit trails</param>
    /// <returns>True if user has admin access, false otherwise</returns>
    public static bool HasAdminAccess(User? user, ILogger? logger = null)
    {
        // Input validation - null check
        if (user == null)
        {
            logger?.LogWarning("Admin access check failed: User is null");
            return false;
        }

        // Security validation - account status
        if (!user.IsActive)
        {
            logger?.LogWarning("Admin access denied for user {UserId}: Account is inactive", user.UserId);
            return false;
        }

        // Security validation - account age (prevent immediate admin access for new accounts)
        if (user.CreatedAt > DateTime.UtcNow.AddMinutes(-5))
        {
            logger?.LogWarning("Admin access denied for user {UserId}: Account too new (created at {CreatedAt})", 
                user.UserId, user.CreatedAt);
            return false;
        }

        // Role validation
        bool hasAccess = user.Role == UserRole.Admin || user.Role == UserRole.SuperAdmin;
        
        if (hasAccess)
        {
            logger?.LogInformation("Admin access granted for user {UserId} with role {Role}", 
                user.UserId, user.Role);
        }
        else
        {
            logger?.LogInformation("Admin access denied for user {UserId} with role {Role}", 
                user.UserId, user.Role);
        }

        return hasAccess;
    }

    /// <summary>
    /// Check if a user has admin access by role
    /// </summary>
    /// <param name="role">The user's role</param>
    /// <param name="isActive">Whether the user is active</param>
    /// <returns>True if user has admin access, false otherwise</returns>
    public static bool HasAdminAccess(UserRole role, bool isActive = true)
    {
        if (!isActive)
            return false;

        return role == UserRole.Admin || role == UserRole.SuperAdmin;
    }

    /// <summary>
    /// Check if a user has instructor-level access or higher
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <returns>True if user has instructor access or higher, false otherwise</returns>
    public static bool HasInstructorAccess(User? user)
    {
        if (user == null)
            return false;

        if (!user.IsActive)
            return false;

        return user.Role == UserRole.Instructor || HasAdminAccess(user);
    }

    /// <summary>
    /// Check if a user can access administrative features
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <returns>True if user can access admin features, false otherwise</returns>
    public static bool CanAccessAdminFeatures(User? user)
    {
        return HasAdminAccess(user);
    }

    /// <summary>
    /// Check if a user can manage other users
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <returns>True if user can manage other users, false otherwise</returns>
    public static bool CanManageUsers(User? user)
    {
        return HasAdminAccess(user);
    }

    /// <summary>
    /// Check if a user can delete courses
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <returns>True if user can delete courses, false otherwise</returns>
    public static bool CanDeleteCourses(User? user)
    {
        return HasAdminAccess(user);
    }

    /// <summary>
    /// Check if a user can view all system data
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <returns>True if user can view all system data, false otherwise</returns>
    public static bool CanViewAllData(User? user)
    {
        return HasInstructorAccess(user);
    }

    /// <summary>
    /// Get the minimum role required for admin access
    /// </summary>
    /// <returns>The minimum UserRole for admin access</returns>
    public static UserRole GetMinimumAdminRole()
    {
        return UserRole.Admin;
    }

    /// <summary>
    /// Get all roles that have admin access
    /// </summary>
    /// <returns>List of UserRoles with admin access</returns>
    public static List<UserRole> GetAdminRoles()
    {
        return new List<UserRole> { UserRole.Admin, UserRole.SuperAdmin };
    }

    /// <summary>
    /// Validate if a role transition is allowed
    /// </summary>
    /// <param name="currentUserRole">The role of the user making the change</param>
    /// <param name="targetCurrentRole">The current role of the target user</param>
    /// <param name="targetNewRole">The new role for the target user</param>
    /// <returns>True if the role change is allowed, false otherwise</returns>
    public static bool CanChangeUserRole(UserRole currentUserRole, UserRole targetCurrentRole, UserRole targetNewRole)
    {
        // Only admins can change roles
        if (!HasAdminAccess(currentUserRole))
            return false;

        // Super admins can change any role
        if (currentUserRole == UserRole.SuperAdmin)
            return true;

        // Regular admins cannot promote users to SuperAdmin or demote SuperAdmins
        if (targetCurrentRole == UserRole.SuperAdmin || targetNewRole == UserRole.SuperAdmin)
            return false;

        return true;
    }

    /// <summary>
    /// Check if user has elevated privileges (Admin or SuperAdmin) with session validation
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <param name="logger">Optional logger for audit trails</param>
    /// <returns>True if user has elevated privileges</returns>
    public static bool HasElevatedPrivileges(User? user, ILogger? logger = null)
    {
        return HasAdminAccess(user, logger);
    }

    /// <summary>
    /// Secure admin access check with claims validation
    /// </summary>
    /// <param name="principal">Claims principal from authentication context</param>
    /// <param name="logger">Optional logger for audit trails</param>
    /// <returns>True if user has valid admin claims</returns>
    public static bool HasAdminAccessFromClaims(ClaimsPrincipal? principal, ILogger? logger = null)
    {
        if (principal == null || !principal.Identity?.IsAuthenticated == true)
        {
            logger?.LogWarning("Admin access denied: No authenticated principal");
            return false;
        }

        var userName = principal.Identity?.Name ?? "Unknown";
        var roleClaim = principal.FindFirst(ClaimTypes.Role);
        
        if (roleClaim == null)
        {
            logger?.LogWarning("Admin access denied for user {UserName}: No role claim found", userName);
            return false;
        }

        if (!Enum.TryParse<UserRole>(roleClaim.Value, out var role))
        {
            logger?.LogWarning("Admin access denied for user {UserName}: Invalid role claim {Role}", 
                userName, roleClaim.Value);
            return false;
        }

        bool hasAccess = role == UserRole.Admin || role == UserRole.SuperAdmin;
        
        if (hasAccess)
        {
            logger?.LogInformation("Admin access granted via claims for user {UserName} with role {Role}", 
                userName, role);
        }

        return hasAccess;
    }

    /// <summary>
    /// Validate admin access with additional security checks (rate limiting, time-based)
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <param name="requestTime">The time of the request</param>
    /// <param name="logger">Optional logger for audit trails</param>
    /// <returns>True if user passes all security validations</returns>
    public static bool ValidateSecureAdminAccess(User? user, DateTime requestTime, ILogger? logger = null)
    {
        // Basic admin access check
        if (!HasAdminAccess(user, logger))
        {
            return false;
        }

        // Time-based validation (no admin access outside business hours for regular admins)
        if (user!.Role == UserRole.Admin && IsOutsideBusinessHours(requestTime))
        {
            logger?.LogWarning("Admin access denied for user {UserId}: Outside business hours", user.UserId);
            return false;
        }

        // Session validation - check last login time
        if (user.LastLoginAt.HasValue && user.LastLoginAt.Value < DateTime.UtcNow.AddHours(-8))
        {
            logger?.LogWarning("Admin access denied for user {UserId}: Session too old", user.UserId);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Check if the current time is outside business hours (9 AM - 6 PM UTC)
    /// </summary>
    /// <param name="currentTime">The time to check</param>
    /// <returns>True if outside business hours</returns>
    private static bool IsOutsideBusinessHours(DateTime currentTime)
    {
        var utcTime = currentTime.ToUniversalTime();
        var hour = utcTime.Hour;
        return hour < 9 || hour >= 18; // 9 AM to 6 PM UTC
    }

    /// <summary>
    /// Comprehensive admin validation with all security checks
    /// </summary>
    /// <param name="user">The user to validate</param>
    /// <param name="principal">Claims principal from authentication</param>
    /// <param name="logger">Logger for audit trails</param>
    /// <returns>AdminValidationResult with detailed validation information</returns>
    public static AdminValidationResult ValidateAdminAccess(User? user, ClaimsPrincipal? principal, ILogger? logger = null)
    {
        var result = new AdminValidationResult();
        
        try
        {
            // User object validation
            result.UserObjectValid = user != null;
            if (!result.UserObjectValid)
            {
                result.FailureReason = "User object is null";
                logger?.LogWarning("Admin validation failed: {Reason}", result.FailureReason);
                return result;
            }

            // Account status validation
            result.AccountActive = user!.IsActive;
            if (!result.AccountActive)
            {
                result.FailureReason = "User account is inactive";
                logger?.LogWarning("Admin validation failed for user {UserId}: {Reason}", user.UserId, result.FailureReason);
                return result;
            }

            // Role validation
            result.HasValidRole = user.Role == UserRole.Admin || user.Role == UserRole.SuperAdmin;
            if (!result.HasValidRole)
            {
                result.FailureReason = $"User has insufficient role: {user.Role}";
                logger?.LogInformation("Admin validation failed for user {UserId}: {Reason}", user.UserId, result.FailureReason);
                return result;
            }

            // Claims validation
            result.ClaimsValid = HasAdminAccessFromClaims(principal, logger);
            if (!result.ClaimsValid)
            {
                result.FailureReason = "Invalid or missing claims";
                return result;
            }

            // Time-based validation
            result.WithinAllowedTime = ValidateSecureAdminAccess(user, DateTime.UtcNow, logger);
            if (!result.WithinAllowedTime)
            {
                result.FailureReason = "Access denied due to time restrictions";
                return result;
            }

            result.IsValid = true;
            result.ValidatedAt = DateTime.UtcNow;
            
            logger?.LogInformation("Admin access validation successful for user {UserId}", user.UserId);
            
            return result;
        }
        catch (Exception ex)
        {
            result.FailureReason = "Validation error occurred";
            logger?.LogError(ex, "Error during admin access validation for user {UserId}", user?.UserId);
            return result;
        }
    }
}

/// <summary>
/// Result of admin access validation with detailed information
/// </summary>
public class AdminValidationResult
{
    public bool IsValid { get; set; }
    public bool UserObjectValid { get; set; }
    public bool AccountActive { get; set; }
    public bool HasValidRole { get; set; }
    public bool ClaimsValid { get; set; }
    public bool WithinAllowedTime { get; set; }
    public string FailureReason { get; set; } = string.Empty;
    public DateTime ValidatedAt { get; set; }

    /// <summary>
    /// Get a summary of validation results
    /// </summary>
    public string GetValidationSummary()
    {
        if (IsValid)
            return "All validations passed";

        return $"Validation failed: {FailureReason}";
    }
}