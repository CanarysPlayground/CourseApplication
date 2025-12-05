using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Domain.Entities;

/// <summary>
/// Represents a student's entry on a course waitlist
/// </summary>
public class WaitlistEntry
{
    /// <summary>
    /// Unique identifier for the waitlist entry
    /// </summary>
    [Key]
    public Guid WaitlistEntryId { get; set; } = Guid.NewGuid();

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
    /// Date and time when the student joined the waitlist
    /// </summary>
    [Required]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Position in the waitlist (1-based)
    /// </summary>
    [Required]
    public int Position { get; set; }

    /// <summary>
    /// Student's notification preference
    /// </summary>
    [Required]
    public NotificationType NotificationPreference { get; set; } = NotificationType.Email;

    /// <summary>
    /// Indicates if the student is still active on the waitlist
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date and time when the student was notified about an available spot
    /// </summary>
    public DateTime? NotifiedAt { get; set; }

    /// <summary>
    /// Notes about the waitlist entry (e.g., admin comments)
    /// </summary>
    [MaxLength(500)]
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
