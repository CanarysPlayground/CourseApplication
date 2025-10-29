using AutoMapper;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Interfaces;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Enums;
using CourseRegistration.Domain.Interfaces;

namespace CourseRegistration.Application.Services;

/// <summary>
/// Service implementation for registration operations
/// </summary>
public class RegistrationService : IRegistrationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the RegistrationService
    /// </summary>
    public RegistrationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Gets all registrations with pagination and filtering
    /// </summary>
    public async Task<PagedResponseDto<RegistrationDto>> GetRegistrationsAsync(
        int page = 1, 
        int pageSize = 10, 
        Guid? studentId = null, 
        Guid? courseId = null, 
        RegistrationStatus? status = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        IEnumerable<Registration> registrations;
        int totalRegistrations;

        if (studentId.HasValue || courseId.HasValue || status.HasValue)
        {
            registrations = await _unitOfWork.Registrations.GetRegistrationsWithFiltersAsync(studentId, courseId, status);
            totalRegistrations = registrations.Count();
            registrations = registrations.Skip((page - 1) * pageSize).Take(pageSize);
        }
        else
        {
            registrations = await _unitOfWork.Registrations.GetPagedAsync(page, pageSize);
            totalRegistrations = await _unitOfWork.Registrations.CountAsync();
        }

        var registrationDtos = _mapper.Map<IEnumerable<RegistrationDto>>(registrations);

        return new PagedResponseDto<RegistrationDto>
        {
            Items = registrationDtos,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalRegistrations,
            TotalPages = (int)Math.Ceiling((double)totalRegistrations / pageSize)
        };
    }

    /// <summary>
    /// Gets a registration by ID
    /// </summary>
    public async Task<RegistrationDto?> GetRegistrationByIdAsync(Guid id)
    {
        var registration = await _unitOfWork.Registrations.GetByIdAsync(id);
        return registration != null ? _mapper.Map<RegistrationDto>(registration) : null;
    }

    /// <summary>
    /// Creates a new registration
    /// </summary>
    public async Task<RegistrationDto> CreateRegistrationAsync(CreateRegistrationDto createRegistrationDto)
    {
        // Validate student exists
        var student = await _unitOfWork.Students.GetByIdAsync(createRegistrationDto.StudentId);
        if (student == null)
        {
            throw new InvalidOperationException("Student not found.");
        }

        // Validate course exists
        var course = await _unitOfWork.Courses.GetByIdAsync(createRegistrationDto.CourseId);
        if (course == null)
        {
            throw new InvalidOperationException("Course not found.");
        }

        // Check if course is active
        if (!course.IsActive)
        {
            throw new InvalidOperationException("Cannot register for an inactive course.");
        }

        // Check if course has not started yet
        if (course.StartDate <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("Cannot register for a course that has already started.");
        }

        // Check if student is already registered for this course
        var existingRegistration = await _unitOfWork.Registrations.IsStudentRegisteredForCourseAsync(
            createRegistrationDto.StudentId, createRegistrationDto.CourseId);
        if (existingRegistration)
        {
            throw new InvalidOperationException("Student is already registered for this course.");
        }

        var registration = _mapper.Map<Registration>(createRegistrationDto);
        registration.Status = RegistrationStatus.Pending;
        registration.RegistrationDate = DateTime.UtcNow;

        await _unitOfWork.Registrations.AddAsync(registration);
        await _unitOfWork.SaveChangesAsync();

        // Get the registration with related entities
        var savedRegistration = await _unitOfWork.Registrations.GetWithDetailsAsync(registration.RegistrationId);
        return _mapper.Map<RegistrationDto>(savedRegistration);
    }

    /// <summary>
    /// Updates registration status
    /// </summary>
    public async Task<RegistrationDto?> UpdateRegistrationStatusAsync(Guid id, UpdateRegistrationStatusDto updateDto)
    {
        var existingRegistration = await _unitOfWork.Registrations.GetWithDetailsAsync(id);
        if (existingRegistration == null)
        {
            return null;
        }

        // Validate status transition
        if (!IsValidStatusTransition(existingRegistration.Status, updateDto.Status))
        {
            throw new InvalidOperationException($"Invalid status transition from {existingRegistration.Status} to {updateDto.Status}.");
        }

        // Validate grade assignment
        if (updateDto.Grade.HasValue && updateDto.Status != RegistrationStatus.Completed)
        {
            throw new InvalidOperationException("Grade can only be assigned to completed registrations.");
        }

        _mapper.Map(updateDto, existingRegistration);
        _unitOfWork.Registrations.Update(existingRegistration);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<RegistrationDto>(existingRegistration);
    }

    /// <summary>
    /// Cancels a registration
    /// </summary>
    public async Task<bool> CancelRegistrationAsync(Guid id)
    {
        var registration = await _unitOfWork.Registrations.GetByIdAsync(id);
        if (registration == null)
        {
            return false;
        }

        if (registration.Status == RegistrationStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel a completed registration.");
        }

        registration.Status = RegistrationStatus.Cancelled;
        _unitOfWork.Registrations.Update(registration);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Gets registrations by student ID
    /// </summary>
    public async Task<IEnumerable<RegistrationDto>> GetRegistrationsByStudentAsync(Guid studentId)
    {
        var registrations = await _unitOfWork.Registrations.GetByStudentIdAsync(studentId);
        return _mapper.Map<IEnumerable<RegistrationDto>>(registrations);
    }

    /// <summary>
    /// Gets registrations by course ID
    /// </summary>
    public async Task<IEnumerable<RegistrationDto>> GetRegistrationsByCourseAsync(Guid courseId)
    {
        var registrations = await _unitOfWork.Registrations.GetByCourseIdAsync(courseId);
        return _mapper.Map<IEnumerable<RegistrationDto>>(registrations);
    }

    /// <summary>
    /// Gets registrations by status
    /// </summary>
    public async Task<IEnumerable<RegistrationDto>> GetRegistrationsByStatusAsync(RegistrationStatus status)
    {
        var registrations = await _unitOfWork.Registrations.GetByStatusAsync(status);
        return _mapper.Map<IEnumerable<RegistrationDto>>(registrations);
    }

    /// <summary>
    /// Checks if a student is already registered for a course
    /// </summary>
    public async Task<bool> IsStudentRegisteredForCourseAsync(Guid studentId, Guid courseId)
    {
        return await _unitOfWork.Registrations.IsStudentRegisteredForCourseAsync(studentId, courseId);
    }

    /// <summary>
    /// Validates if a status transition is allowed
    /// </summary>
    private static bool IsValidStatusTransition(RegistrationStatus currentStatus, RegistrationStatus newStatus)
    {
        return currentStatus switch
        {
            RegistrationStatus.Pending => newStatus is RegistrationStatus.Confirmed or RegistrationStatus.Cancelled,
            RegistrationStatus.Confirmed => newStatus is RegistrationStatus.Completed or RegistrationStatus.Cancelled,
            RegistrationStatus.Cancelled => false, // Cannot change from cancelled
            RegistrationStatus.Completed => false, // Cannot change from completed
            _ => false
        };
    }
}