using CourseRegistration.Application.DTOs;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Application.Interfaces;

/// <summary>
/// Service interface for registration operations
/// </summary>
public interface IRegistrationService
{
    /// <summary>
    /// Gets all registrations with pagination and filtering
    /// </summary>
    Task<PagedResponseDto<RegistrationDto>> GetRegistrationsAsync(
        int page = 1, 
        int pageSize = 10, 
        Guid? studentId = null, 
        Guid? courseId = null, 
        RegistrationStatus? status = null);

    /// <summary>
    /// Gets a registration by ID
    /// </summary>
    Task<RegistrationDto?> GetRegistrationByIdAsync(Guid id);

    /// <summary>
    /// Creates a new registration
    /// </summary>
    Task<RegistrationDto> CreateRegistrationAsync(CreateRegistrationDto createRegistrationDto);

    /// <summary>
    /// Updates registration status
    /// </summary>
    Task<RegistrationDto?> UpdateRegistrationStatusAsync(Guid id, UpdateRegistrationStatusDto updateDto);

    /// <summary>
    /// Cancels a registration
    /// </summary>
    Task<bool> CancelRegistrationAsync(Guid id);

    /// <summary>
    /// Gets registrations by student ID
    /// </summary>
    Task<IEnumerable<RegistrationDto>> GetRegistrationsByStudentAsync(Guid studentId);

    /// <summary>
    /// Gets registrations by course ID
    /// </summary>
    Task<IEnumerable<RegistrationDto>> GetRegistrationsByCourseAsync(Guid courseId);

    /// <summary>
    /// Gets registrations by status
    /// </summary>
    Task<IEnumerable<RegistrationDto>> GetRegistrationsByStatusAsync(RegistrationStatus status);

    /// <summary>
    /// Checks if a student is already registered for a course
    /// </summary>
    Task<bool> IsStudentRegisteredForCourseAsync(Guid studentId, Guid courseId);
}