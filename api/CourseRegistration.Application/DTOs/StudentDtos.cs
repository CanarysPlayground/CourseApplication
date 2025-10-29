namespace CourseRegistration.Application.DTOs;

/// <summary>
/// Data transfer object for creating a new student
/// </summary>
public class CreateStudentDto
{
    /// <summary>
    /// Student's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Student's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Student's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Student's phone number (optional)
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Student's date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing student
/// </summary>
public class UpdateStudentDto
{
    /// <summary>
    /// Student's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Student's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Student's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Student's phone number (optional)
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Student's date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }
}

/// <summary>
/// Data transfer object for student response
/// </summary>
public class StudentDto
{
    /// <summary>
    /// Student's unique identifier
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// Student's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Student's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Student's full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Student's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Student's phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Student's date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Student's age
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Date when the student was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date when the student was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}