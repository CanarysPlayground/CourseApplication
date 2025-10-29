namespace CourseRegistration.Application.DTOs;

/// <summary>
/// Data transfer object for creating a new course
/// </summary>
public class CreateCourseDto
{
    /// <summary>
    /// Course name
    /// </summary>
    public string CourseName { get; set; } = string.Empty;

    /// <summary>
    /// Course description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Name of the course instructor
    /// </summary>
    public string InstructorName { get; set; } = string.Empty;

    /// <summary>
    /// Course start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Course end date
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Course schedule
    /// </summary>
    public string Schedule { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for updating an existing course
/// </summary>
public class UpdateCourseDto
{
    /// <summary>
    /// Course name
    /// </summary>
    public string CourseName { get; set; } = string.Empty;

    /// <summary>
    /// Course description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Name of the course instructor
    /// </summary>
    public string InstructorName { get; set; } = string.Empty;

    /// <summary>
    /// Course start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Course end date
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Course schedule
    /// </summary>
    public string Schedule { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for course response
/// </summary>
public class CourseDto
{
    /// <summary>
    /// Course's unique identifier
    /// </summary>
    public Guid CourseId { get; set; }

    /// <summary>
    /// Course name
    /// </summary>
    public string CourseName { get; set; } = string.Empty;

    /// <summary>
    /// Course description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Name of the course instructor
    /// </summary>
    public string InstructorName { get; set; } = string.Empty;

    /// <summary>
    /// Course start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Course end date
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Course schedule
    /// </summary>
    public string Schedule { get; set; } = string.Empty;

    /// <summary>
    /// Current enrollment count
    /// </summary>
    public int CurrentEnrollment { get; set; }

    /// <summary>
    /// Indicates if the course is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Date when the course was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date when the course was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}