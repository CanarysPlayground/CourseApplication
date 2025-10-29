using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseRegistration.Domain.Entities;

/// <summary>
/// Represents a course in the course registration system
/// </summary>
public class Course
{
    /// <summary>
    /// Unique identifier for the course
    /// </summary>
    [Key]
    public Guid CourseId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Course name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string CourseName { get; set; } = string.Empty;

    /// <summary>
    /// Course description
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Name of the course instructor
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string InstructorName { get; set; } = string.Empty;

    /// <summary>
    /// Course start date
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Course end date
    /// </summary>
    [Required]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Course schedule (e.g., "MWF 10:00-11:00")
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Schedule { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the course is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date when the course record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the course record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property for course registrations
    /// </summary>
    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    /// <summary>
    /// Computed property for current enrollment count
    /// </summary>
    [NotMapped]
    public int CurrentEnrollment => Registrations?.Count(r => r.Status == Enums.RegistrationStatus.Confirmed) ?? 0;
}