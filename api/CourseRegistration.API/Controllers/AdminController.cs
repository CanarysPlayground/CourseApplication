using Microsoft.AspNetCore.Mvc;
using CourseRegistration.Application.Services;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Enums;
using CourseRegistration.API.Attributes;

namespace CourseRegistration.API.Controllers;

/// <summary>
/// Controller for administrative operations and user management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly AuthorizationService _authorizationService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        AuthorizationService authorizationService,
        ILogger<AdminController> logger)
    {
        _authorizationService = authorizationService;
        _logger = logger;
    }

    /// <summary>
    /// Check if the current user has admin access
    /// </summary>
    /// <returns>Boolean indicating admin access status</returns>
    [HttpGet("check-admin-access")]
    public IActionResult CheckAdminAccess()
    {
        try
        {
            // Create a sample user for demonstration (in real app, get from authentication context)
            var sampleUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = "testadmin",
                Email = "admin@example.com",
                FirstName = "Admin",
                LastName = "User",
                Role = UserRole.Admin,
                IsActive = true
            };

            var hasAccess = AdminAccessChecker.HasAdminAccess(sampleUser);
            
            _logger.LogInformation("Admin access check for user {UserId}: {HasAccess}", 
                sampleUser.UserId, hasAccess);

            return Ok(new
            {
                hasAdminAccess = hasAccess,
                userRole = sampleUser.Role.ToString(),
                userId = sampleUser.UserId,
                message = hasAccess ? "User has admin access" : "User does not have admin access"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking admin access");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Check admin access by role (demonstration endpoint)
    /// </summary>
    /// <param name="role">The role to check</param>
    /// <returns>Boolean indicating if the role has admin access</returns>
    [HttpGet("check-role-access/{role}")]
    public IActionResult CheckRoleAccess(string role)
    {
        try
        {
            if (!Enum.TryParse<UserRole>(role, true, out var userRole))
            {
                return BadRequest(new { message = "Invalid role specified" });
            }

            var hasAccess = AdminAccessChecker.HasAdminAccess(userRole);
            
            return Ok(new
            {
                role = userRole.ToString(),
                hasAdminAccess = hasAccess,
                message = $"Role '{userRole}' {(hasAccess ? "has" : "does not have")} admin access"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role access for role {Role}", role);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get all admin roles
    /// </summary>
    /// <returns>List of roles that have admin access</returns>
    [HttpGet("admin-roles")]
    public IActionResult GetAdminRoles()
    {
        try
        {
            var adminRoles = AdminAccessChecker.GetAdminRoles();
            var minimumRole = AdminAccessChecker.GetMinimumAdminRole();

            return Ok(new
            {
                adminRoles = adminRoles.Select(r => r.ToString()),
                minimumAdminRole = minimumRole.ToString(),
                message = "Admin roles retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving admin roles");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Protected endpoint that requires admin access
    /// </summary>
    /// <returns>Success message if user has admin access</returns>
    [HttpGet("protected-admin-endpoint")]
    [RequireAdminAccess]
    public IActionResult ProtectedAdminEndpoint()
    {
        return Ok(new
        {
            message = "Successfully accessed admin-only endpoint",
            timestamp = DateTime.UtcNow,
            userHasAccess = true
        });
    }

    /// <summary>
    /// Protected endpoint that requires instructor access
    /// </summary>
    /// <returns>Success message if user has instructor access</returns>
    [HttpGet("protected-instructor-endpoint")]
    [RequireInstructorAccess]
    public IActionResult ProtectedInstructorEndpoint()
    {
        return Ok(new
        {
            message = "Successfully accessed instructor-level endpoint",
            timestamp = DateTime.UtcNow,
            userHasAccess = true
        });
    }

    /// <summary>
    /// Validate role change permissions
    /// </summary>
    /// <param name="currentRole">Current user's role</param>
    /// <param name="targetCurrentRole">Target user's current role</param>
    /// <param name="targetNewRole">Target user's new role</param>
    /// <returns>Boolean indicating if role change is allowed</returns>
    [HttpPost("validate-role-change")]
    public IActionResult ValidateRoleChange([FromBody] RoleChangeRequest request)
    {
        try
        {
            if (!Enum.TryParse<UserRole>(request.CurrentRole, true, out var currentRole) ||
                !Enum.TryParse<UserRole>(request.TargetCurrentRole, true, out var targetCurrentRole) ||
                !Enum.TryParse<UserRole>(request.TargetNewRole, true, out var targetNewRole))
            {
                return BadRequest(new { message = "Invalid role specified" });
            }

            var canChange = AdminAccessChecker.CanChangeUserRole(currentRole, targetCurrentRole, targetNewRole);

            return Ok(new
            {
                canChangeRole = canChange,
                currentRole = currentRole.ToString(),
                targetCurrentRole = targetCurrentRole.ToString(),
                targetNewRole = targetNewRole.ToString(),
                message = canChange ? "Role change is allowed" : "Role change is not allowed"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating role change");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

/// <summary>
/// Request model for role change validation
/// </summary>
public class RoleChangeRequest
{
    public string CurrentRole { get; set; } = string.Empty;
    public string TargetCurrentRole { get; set; } = string.Empty;
    public string TargetNewRole { get; set; } = string.Empty;
}