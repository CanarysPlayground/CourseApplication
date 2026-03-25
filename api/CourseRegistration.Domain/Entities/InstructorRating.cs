using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseRegistration.Domain.Entities;

/// <summary>
/// Represents a student's rating and review of an instructor for a specific course
/// </summary>
public class InstructorRating
{
    /// <summary>
    /// Unique identifier for the rating
    /// </summary>
    [Key]
    public Guid RatingId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Foreign key to the course
    /// </summary>
    [Required]
    public Guid CourseId { get; set; }

    /// <summary>
    /// Foreign key to the student who submitted the rating
    /// </summary>
    [Required]
    public Guid StudentId { get; set; }

    /// <summary>
    /// Rating value (1-5 scale)
    /// </summary>
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    /// <summary>
    /// Optional comment/review text
    /// </summary>
    [MaxLength(1000)]
    public string? Comment { get; set; }

    /// <summary>
    /// Date when the rating was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the rating was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property to the course
    /// </summary>
    [ForeignKey(nameof(CourseId))]
    public virtual Course Course { get; set; } = null!;

    /// <summary>
    /// Navigation property to the student
    /// </summary>
    [ForeignKey(nameof(StudentId))]
    public virtual Student Student { get; set; } = null!;
}
