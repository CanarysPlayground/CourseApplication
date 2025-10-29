using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Domain.Entities;

/// <summary>
/// Represents a student's registration for a course
/// </summary>
public class Registration
{
    /// <summary>
    /// Unique identifier for the registration
    /// </summary>
    [Key]
    public Guid RegistrationId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Foreign key to the student
    /// </summary>
    [Required]
    public Guid StudentId { get; set; }

    /// <summary>
    /// Foreign key to the course
    /// </summary>
    [Required]
    public Guid CourseId { get; set; }

    /// <summary>
    /// Date when the registration was created
    /// </summary>
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Current status of the registration
    /// </summary>
    [Required]
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;

    /// <summary>
    /// Grade assigned to the student (optional)
    /// </summary>
    public Grade? Grade { get; set; }

    /// <summary>
    /// Additional notes about the registration
    /// </summary>
    [MaxLength(200)]
    public string? Notes { get; set; }

    /// <summary>
    /// Navigation property to the student
    /// </summary>
    [ForeignKey(nameof(StudentId))]
    public virtual Student Student { get; set; } = null!;

    /// <summary>
    /// Navigation property to the course
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public virtual Course Course { get; set; } = null!;
}