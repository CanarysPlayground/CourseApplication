using CourseRegistration.Application.DTOs;

namespace CourseRegistration.Application.Interfaces;

/// <summary>
/// Service interface for student operations
/// </summary>
public interface IStudentService
{
    /// <summary>
    /// Gets all students with pagination
    /// </summary>
    Task<PagedResponseDto<StudentDto>> GetStudentsAsync(int page = 1, int pageSize = 10);

    /// <summary>
    /// Gets a student by ID
    /// </summary>
    Task<StudentDto?> GetStudentByIdAsync(Guid id);

    /// <summary>
    /// Gets a student by email
    /// </summary>
    Task<StudentDto?> GetStudentByEmailAsync(string email);

    /// <summary>
    /// Creates a new student
    /// </summary>
    Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto);

    /// <summary>
    /// Updates an existing student
    /// </summary>
    Task<StudentDto?> UpdateStudentAsync(Guid id, UpdateStudentDto updateStudentDto);

    /// <summary>
    /// Deletes a student (soft delete)
    /// </summary>
    Task<bool> DeleteStudentAsync(Guid id);

    /// <summary>
    /// Searches students by name
    /// </summary>
    Task<IEnumerable<StudentDto>> SearchStudentsAsync(string searchTerm);

    /// <summary>
    /// Gets student's registrations
    /// </summary>
    Task<IEnumerable<RegistrationDto>> GetStudentRegistrationsAsync(Guid studentId);
}