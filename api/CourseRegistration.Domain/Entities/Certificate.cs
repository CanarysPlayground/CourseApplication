using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Domain.Entities;

/// <summary>
/// Represents a certificate issued to a student for completing a course
/// </summary>
public class Certificate
{
    /// <summary>
    /// Unique identifier for the certificate
    /// </summary>
    [Key]
    public Guid CertificateId { get; set; } = Guid.NewGuid();

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
    /// Date when the certificate was issued
    /// </summary>
    [Required]
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Final grade achieved in the course
    /// </summary>
    [Required]
    public Grade FinalGrade { get; set; }

    /// <summary>
    /// Certificate number for tracking
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string CertificateNumber { get; set; } = string.Empty;

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

    /// <summary>
    /// Additional remarks or achievements
    /// </summary>
    [MaxLength(200)]
    public string? Remarks { get; set; }

    /// <summary>
    /// Digital signature or verification code
    /// </summary>
    [MaxLength(100)]
    public string? DigitalSignature { get; set; }
}