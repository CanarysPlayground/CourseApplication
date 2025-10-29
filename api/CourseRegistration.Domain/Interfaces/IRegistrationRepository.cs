using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Domain.Interfaces;

/// <summary>
/// Registration repository interface with specific operations
/// </summary>
public interface IRegistrationRepository : IRepository<Registration>
{
    /// <summary>
    /// Gets registrations for a specific student asynchronously
    /// </summary>
    Task<IEnumerable<Registration>> GetByStudentIdAsync(Guid studentId);

    /// <summary>
    /// Gets registrations for a specific course asynchronously
    /// </summary>
    Task<IEnumerable<Registration>> GetByCourseIdAsync(Guid courseId);

    /// <summary>
    /// Gets registrations by status asynchronously
    /// </summary>
    Task<IEnumerable<Registration>> GetByStatusAsync(RegistrationStatus status);

    /// <summary>
    /// Checks if a student is already registered for a course asynchronously
    /// </summary>
    Task<bool> IsStudentRegisteredForCourseAsync(Guid studentId, Guid courseId);

    /// <summary>
    /// Gets registration with student and course details asynchronously
    /// </summary>
    Task<Registration?> GetWithDetailsAsync(Guid registrationId);

    /// <summary>
    /// Gets registrations with filtering options asynchronously
    /// </summary>
    Task<IEnumerable<Registration>> GetRegistrationsWithFiltersAsync(
        Guid? studentId = null, 
        Guid? courseId = null, 
        RegistrationStatus? status = null);
}