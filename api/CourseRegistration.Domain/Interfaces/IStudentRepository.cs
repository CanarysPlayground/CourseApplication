using CourseRegistration.Domain.Entities;

namespace CourseRegistration.Domain.Interfaces;

/// <summary>
/// Student repository interface with specific operations
/// </summary>
public interface IStudentRepository : IRepository<Student>
{
    /// <summary>
    /// Gets a student by email address asynchronously
    /// </summary>
    Task<Student?> GetByEmailAsync(string email);

    /// <summary>
    /// Gets students with their registrations asynchronously
    /// </summary>
    Task<Student?> GetWithRegistrationsAsync(Guid studentId);

    /// <summary>
    /// Searches students by name asynchronously
    /// </summary>
    Task<IEnumerable<Student>> SearchByNameAsync(string searchTerm);

    /// <summary>
    /// Gets active students asynchronously
    /// </summary>
    Task<IEnumerable<Student>> GetActiveStudentsAsync();
}