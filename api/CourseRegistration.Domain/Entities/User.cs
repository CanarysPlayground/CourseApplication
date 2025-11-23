using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Domain.Entities;

/// <summary>
/// Represents a user in the system with authentication and authorization information
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    [Key]
    public Guid UserId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// User's username (must be unique)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// User's email address (must be unique)
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's first name
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's role in the system
    /// </summary>
    [Required]
    public UserRole Role { get; set; } = UserRole.Student;

    /// <summary>
    /// Indicates if the user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date when the user account was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the user account was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the user last logged in
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Computed property for user's full name
    /// </summary>
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Check if the user has admin access
    /// </summary>
    /// <returns>True if user has admin or super admin role</returns>
    [NotMapped]
    public bool HasAdminAccess => Role == UserRole.Admin || Role == UserRole.SuperAdmin;

    /// <summary>
    /// Check if the user has instructor access
    /// </summary>
    /// <returns>True if user has instructor, admin, or super admin role</returns>
    [NotMapped]
    public bool HasInstructorAccess => Role == UserRole.Instructor || HasAdminAccess;
}