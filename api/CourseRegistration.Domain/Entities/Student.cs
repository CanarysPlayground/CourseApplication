using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseRegistration.Domain.Entities;

/// <summary>
/// Represents a student in the course registration system
/// </summary>
public class Student
{
    /// <summary>
    /// Unique identifier for the student
    /// </summary>
    [Key]
    public Guid StudentId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Student's first name
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Student's last name
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Student's email address (must be unique)
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Student's phone number (optional)
    /// </summary>
    [Phone]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Student's date of birth
    /// </summary>
    [Required]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Date when the student record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the student record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indicates if the student record is active (soft delete)
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property for student's course registrations
    /// </summary>
    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    /// <summary>
    /// Computed property for student's full name
    /// </summary>
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Computed property to calculate student's age
    /// </summary>
    [NotMapped]
    public int Age => DateTime.UtcNow.Year - DateOfBirth.Year - 
                     (DateTime.UtcNow.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
}