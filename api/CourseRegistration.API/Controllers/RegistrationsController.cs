using Microsoft.AspNetCore.Mvc;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Interfaces;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.API.Controllers;

/// <summary>
/// Controller for registration management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RegistrationsController : ControllerBase
{
    private readonly IRegistrationService _registrationService;
    private readonly ILogger<RegistrationsController> _logger;

    /// <summary>
    /// Initializes a new instance of the RegistrationsController
    /// </summary>
    public RegistrationsController(IRegistrationService registrationService, ILogger<RegistrationsController> logger)
    {
        _registrationService = registrationService ?? throw new ArgumentNullException(nameof(registrationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all registrations with pagination and filtering
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="studentId">Filter by student ID</param>
    /// <param name="courseId">Filter by course ID</param>
    /// <param name="status">Filter by registration status</param>
    /// <returns>Paginated list of registrations</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<PagedResponseDto<RegistrationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponseDto<PagedResponseDto<RegistrationDto>>>> GetRegistrations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? studentId = null,
        [FromQuery] Guid? courseId = null,
        [FromQuery] RegistrationStatus? status = null)
    {
        _logger.LogInformation("Getting registrations with page: {Page}, pageSize: {PageSize}, studentId: {StudentId}, courseId: {CourseId}, status: {Status}", 
            page, pageSize, studentId, courseId, status);
        
        var result = await _registrationService.GetRegistrationsAsync(page, pageSize, studentId, courseId, status);
        return Ok(ApiResponseDto<PagedResponseDto<RegistrationDto>>.SuccessResponse(result, "Registrations retrieved successfully"));
    }

    /// <summary>
    /// Gets a registration by ID
    /// </summary>
    /// <param name="id">Registration ID</param>
    /// <returns>Registration details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<RegistrationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponseDto<RegistrationDto>>> GetRegistration(Guid id)
    {
        _logger.LogInformation("Getting registration with ID: {RegistrationId}", id);
        
        var registration = await _registrationService.GetRegistrationByIdAsync(id);
        if (registration == null)
        {
            return NotFound(ApiResponseDto<RegistrationDto>.ErrorResponse("Registration not found"));
        }

        return Ok(ApiResponseDto<RegistrationDto>.SuccessResponse(registration, "Registration retrieved successfully"));
    }

    /// <summary>
    /// Registers a student for a course
    /// </summary>
    /// <param name="createRegistrationDto">Registration details</param>
    /// <returns>Created registration</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<RegistrationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponseDto<RegistrationDto>>> CreateRegistration([FromBody] CreateRegistrationDto createRegistrationDto)
    {
        _logger.LogInformation("Creating registration for student: {StudentId}, course: {CourseId}", 
            createRegistrationDto.StudentId, createRegistrationDto.CourseId);
        
        var registration = await _registrationService.CreateRegistrationAsync(createRegistrationDto);
        
        return CreatedAtAction(
            nameof(GetRegistration),
            new { id = registration.RegistrationId },
            ApiResponseDto<RegistrationDto>.SuccessResponse(registration, "Registration created successfully"));
    }

    /// <summary>
    /// Updates registration status
    /// </summary>
    /// <param name="id">Registration ID</param>
    /// <param name="updateDto">Status update details</param>
    /// <returns>Updated registration</returns>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponseDto<RegistrationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponseDto<RegistrationDto>>> UpdateRegistrationStatus(
        Guid id, 
        [FromBody] UpdateRegistrationStatusDto updateDto)
    {
        _logger.LogInformation("Updating registration status for ID: {RegistrationId} to status: {Status}", id, updateDto.Status);
        
        var registration = await _registrationService.UpdateRegistrationStatusAsync(id, updateDto);
        if (registration == null)
        {
            return NotFound(ApiResponseDto<RegistrationDto>.ErrorResponse("Registration not found"));
        }

        return Ok(ApiResponseDto<RegistrationDto>.SuccessResponse(registration, "Registration status updated successfully"));
    }

    /// <summary>
    /// Cancels a registration
    /// </summary>
    /// <param name="id">Registration ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponseDto<object>>> CancelRegistration(Guid id)
    {
        _logger.LogInformation("Cancelling registration with ID: {RegistrationId}", id);
        
        var result = await _registrationService.CancelRegistrationAsync(id);
        if (!result)
        {
            return NotFound(ApiResponseDto<object>.ErrorResponse("Registration not found"));
        }

        return Ok(ApiResponseDto<object>.SuccessResponse(null, "Registration cancelled successfully"));
    }

    /// <summary>
    /// Gets registrations by student ID
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <returns>List of student's registrations</returns>
    [HttpGet("student/{studentId:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<RegistrationDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<RegistrationDto>>>> GetRegistrationsByStudent(Guid studentId)
    {
        _logger.LogInformation("Getting registrations for student: {StudentId}", studentId);
        
        var registrations = await _registrationService.GetRegistrationsByStudentAsync(studentId);
        return Ok(ApiResponseDto<IEnumerable<RegistrationDto>>.SuccessResponse(registrations, "Student registrations retrieved successfully"));
    }

    /// <summary>
    /// Gets registrations by course ID
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>List of course registrations</returns>
    [HttpGet("course/{courseId:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<RegistrationDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<RegistrationDto>>>> GetRegistrationsByCourse(Guid courseId)
    {
        _logger.LogInformation("Getting registrations for course: {CourseId}", courseId);
        
        var registrations = await _registrationService.GetRegistrationsByCourseAsync(courseId);
        return Ok(ApiResponseDto<IEnumerable<RegistrationDto>>.SuccessResponse(registrations, "Course registrations retrieved successfully"));
    }

    /// <summary>
    /// Gets registrations by status
    /// </summary>
    /// <param name="status">Registration status</param>
    /// <returns>List of registrations with the specified status</returns>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<RegistrationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<RegistrationDto>>>> GetRegistrationsByStatus(RegistrationStatus status)
    {
        _logger.LogInformation("Getting registrations with status: {Status}", status);
        
        var registrations = await _registrationService.GetRegistrationsByStatusAsync(status);
        return Ok(ApiResponseDto<IEnumerable<RegistrationDto>>.SuccessResponse(registrations, $"Registrations with status {status} retrieved successfully"));
    }

    /// <summary>
    /// Checks if a student is already registered for a course
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="courseId">Course ID</param>
    /// <returns>Boolean indicating if student is registered</returns>
    [HttpGet("check")]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponseDto<bool>>> CheckRegistration(
        [FromQuery] Guid studentId, 
        [FromQuery] Guid courseId)
    {
        _logger.LogInformation("Checking registration for student: {StudentId}, course: {CourseId}", studentId, courseId);
        
        if (studentId == Guid.Empty || courseId == Guid.Empty)
        {
            return BadRequest(ApiResponseDto<bool>.ErrorResponse("Both studentId and courseId are required"));
        }

        var isRegistered = await _registrationService.IsStudentRegisteredForCourseAsync(studentId, courseId);
        return Ok(ApiResponseDto<bool>.SuccessResponse(isRegistered, "Registration check completed"));
    }
}