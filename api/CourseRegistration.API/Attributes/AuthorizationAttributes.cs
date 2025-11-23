using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CourseRegistration.Application.Services;
using CourseRegistration.Domain.Entities;
using System.Security.Claims;

namespace CourseRegistration.API.Attributes;

/// <summary>
/// Authorization filter attribute to check if user has admin access
/// </summary>
public class RequireAdminAccessAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authorizationService = context.HttpContext.RequestServices
            .GetService<AuthorizationService>();

        if (authorizationService == null)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return;
        }

        // Get user from context (this would typically come from JWT token or session)
        var user = GetCurrentUser(context.HttpContext);
        
        if (user == null)
        {
            context.Result = new UnauthorizedObjectResult(new { message = "User not authenticated" });
            return;
        }

        if (!authorizationService.HasAdminAccess(user))
        {
            context.Result = new ForbidResult();
            return;
        }
    }

    private User? GetCurrentUser(HttpContext httpContext)
    {
        // This is a simplified implementation
        // In a real application, you would get the user from JWT token claims or session
        
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return null;

        var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role);
        if (roleClaim == null || !Enum.TryParse<Domain.Enums.UserRole>(roleClaim.Value, out var role))
            return null;

        var emailClaim = httpContext.User.FindFirst(ClaimTypes.Email);
        var nameClaim = httpContext.User.FindFirst(ClaimTypes.Name);

        return new User
        {
            UserId = userId,
            Role = role,
            Email = emailClaim?.Value ?? "",
            Username = nameClaim?.Value ?? "",
            FirstName = "Current",
            LastName = "User",
            IsActive = true
        };
    }
}

/// <summary>
/// Authorization filter attribute to check if user has instructor access
/// </summary>
public class RequireInstructorAccessAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authorizationService = context.HttpContext.RequestServices
            .GetService<AuthorizationService>();

        if (authorizationService == null)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return;
        }

        var user = GetCurrentUser(context.HttpContext);
        
        if (user == null)
        {
            context.Result = new UnauthorizedObjectResult(new { message = "User not authenticated" });
            return;
        }

        if (!authorizationService.HasInstructorAccess(user))
        {
            context.Result = new ForbidResult();
            return;
        }
    }

    private User? GetCurrentUser(HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return null;

        var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role);
        if (roleClaim == null || !Enum.TryParse<Domain.Enums.UserRole>(roleClaim.Value, out var role))
            return null;

        var emailClaim = httpContext.User.FindFirst(ClaimTypes.Email);
        var nameClaim = httpContext.User.FindFirst(ClaimTypes.Name);

        return new User
        {
            UserId = userId,
            Role = role,
            Email = emailClaim?.Value ?? "",
            Username = nameClaim?.Value ?? "",
            FirstName = "Current",
            LastName = "User",
            IsActive = true
        };
    }
}