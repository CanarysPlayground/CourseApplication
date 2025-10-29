using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Application.DTOs;

/// <summary>
/// Data transfer object for creating a new registration
/// </summary>
public class CreateRegistrationDto
{
    /// <summary>
    /// Student ID for the registration
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// Course ID for the registration
    /// </summary>
    public Guid CourseId { get; set; }

    /// <summary>
    /// Additional notes about the registration
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Data transfer object for updating registration status
/// </summary>
public class UpdateRegistrationStatusDto
{
    /// <summary>
    /// New registration status
    /// </summary>
    public RegistrationStatus Status { get; set; }

    /// <summary>
    /// Grade assigned to the student (optional)
    /// </summary>
    public Grade? Grade { get; set; }

    /// <summary>
    /// Additional notes about the status change
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Data transfer object for registration response
/// </summary>
public class RegistrationDto
{
    /// <summary>
    /// Registration's unique identifier
    /// </summary>
    public Guid RegistrationId { get; set; }

    /// <summary>
    /// Student ID
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// Course ID
    /// </summary>
    public Guid CourseId { get; set; }

    /// <summary>
    /// Student information
    /// </summary>
    public StudentDto? Student { get; set; }

    /// <summary>
    /// Course information
    /// </summary>
    public CourseDto? Course { get; set; }

    /// <summary>
    /// Registration date
    /// </summary>
    public DateTime RegistrationDate { get; set; }

    /// <summary>
    /// Current registration status
    /// </summary>
    public RegistrationStatus Status { get; set; }

    /// <summary>
    /// Grade assigned to the student
    /// </summary>
    public Grade? Grade { get; set; }

    /// <summary>
    /// Additional notes about the registration
    /// </summary>
    public string? Notes { get; set; }
}