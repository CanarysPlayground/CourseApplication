using Microsoft.AspNetCore.Mvc;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Interfaces;

namespace CourseRegistration.API.Controllers;

/// <summary>
/// Controller for waitlist management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WaitlistController : ControllerBase
{
    private readonly IWaitlistService _waitlistService;
    private readonly ILogger<WaitlistController> _logger;

    /// <summary>
    /// Initializes a new instance of the WaitlistController
    /// </summary>
    public WaitlistController(IWaitlistService waitlistService, ILogger<WaitlistController> logger)
    {
        _waitlistService = waitlistService ?? throw new ArgumentNullException(nameof(waitlistService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Adds a student to a course waitlist
    /// </summary>
    /// <param name="createWaitlistEntryDto">Waitlist entry creation data</param>
    /// <returns>Created waitlist entry</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<WaitlistEntryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponseDto<WaitlistEntryDto>>> JoinWaitlist([FromBody] CreateWaitlistEntryDto createWaitlistEntryDto)
    {
        _logger.LogInformation("Student {StudentId} attempting to join waitlist for course {CourseId}", 
            createWaitlistEntryDto.StudentId, createWaitlistEntryDto.CourseId);

        try
        {
            var waitlistEntry = await _waitlistService.JoinWaitlistAsync(createWaitlistEntryDto);
            return CreatedAtAction(
                nameof(GetWaitlistEntry),
                new { id = waitlistEntry.WaitlistEntryId },
                new ApiResponseDto<WaitlistEntryDto>
                {
                    Success = true,
                    Message = $"Successfully joined waitlist at position {waitlistEntry.Position}",
                    Data = waitlistEntry
                });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Failed to join waitlist: {Message}", ex.Message);
            return BadRequest(new ApiResponseDto<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Gets a specific waitlist entry by ID
    /// </summary>
    /// <param name="id">Waitlist entry ID</param>
    /// <returns>Waitlist entry details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseDto<WaitlistEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponseDto<WaitlistEntryDto>>> GetWaitlistEntry(Guid id)
    {
        _logger.LogInformation("Getting waitlist entry {Id}", id);

        var waitlistEntry = await _waitlistService.GetWaitlistEntryAsync(id);

        if (waitlistEntry == null)
        {
            return NotFound(new ApiResponseDto<object>
            {
                Success = false,
                Message = "Waitlist entry not found"
            });
        }

        return Ok(new ApiResponseDto<WaitlistEntryDto>
        {
            Success = true,
            Data = waitlistEntry
        });
    }

    /// <summary>
    /// Gets active waitlist entries for a specific course
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>List of waitlist entries for the course</returns>
    [HttpGet("course/{courseId}")]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<WaitlistEntryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<WaitlistEntryDto>>>> GetCourseWaitlist(Guid courseId)
    {
        _logger.LogInformation("Getting waitlist for course {CourseId}", courseId);

        var waitlistEntries = await _waitlistService.GetCourseWaitlistAsync(courseId);

        return Ok(new ApiResponseDto<IEnumerable<WaitlistEntryDto>>
        {
            Success = true,
            Data = waitlistEntries,
            Message = $"Found {waitlistEntries.Count()} entries in waitlist"
        });
    }

    /// <summary>
    /// Gets active waitlist entries for a specific student
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <returns>List of waitlist entries for the student</returns>
    [HttpGet("student/{studentId}")]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<WaitlistEntryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<WaitlistEntryDto>>>> GetStudentWaitlists(Guid studentId)
    {
        _logger.LogInformation("Getting waitlists for student {StudentId}", studentId);

        var waitlistEntries = await _waitlistService.GetStudentWaitlistsAsync(studentId);

        return Ok(new ApiResponseDto<IEnumerable<WaitlistEntryDto>>
        {
            Success = true,
            Data = waitlistEntries,
            Message = $"Student is on {waitlistEntries.Count()} waitlist(s)"
        });
    }

    /// <summary>
    /// Removes a student from a waitlist
    /// </summary>
    /// <param name="id">Waitlist entry ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponseDto<object>>> LeaveWaitlist(Guid id)
    {
        _logger.LogInformation("Attempting to remove waitlist entry {Id}", id);

        var result = await _waitlistService.LeaveWaitlistAsync(id);

        if (!result)
        {
            return NotFound(new ApiResponseDto<object>
            {
                Success = false,
                Message = "Waitlist entry not found or already inactive"
            });
        }

        return Ok(new ApiResponseDto<object>
        {
            Success = true,
            Message = "Successfully left the waitlist"
        });
    }

    /// <summary>
    /// Updates a waitlist entry (admin function)
    /// </summary>
    /// <param name="id">Waitlist entry ID</param>
    /// <param name="updateDto">Update data</param>
    /// <returns>Updated waitlist entry</returns>
    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(ApiResponseDto<WaitlistEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponseDto<WaitlistEntryDto>>> UpdateWaitlistEntry(
        Guid id,
        [FromBody] UpdateWaitlistEntryDto updateDto)
    {
        _logger.LogInformation("Updating waitlist entry {Id}", id);

        try
        {
            var updatedEntry = await _waitlistService.UpdateWaitlistEntryAsync(id, updateDto);

            if (updatedEntry == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Waitlist entry not found"
                });
            }

            return Ok(new ApiResponseDto<WaitlistEntryDto>
            {
                Success = true,
                Message = "Waitlist entry updated successfully",
                Data = updatedEntry
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Failed to update waitlist entry: {Message}", ex.Message);
            return BadRequest(new ApiResponseDto<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Clears the entire waitlist for a course (admin function)
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("course/{courseId}/clear")]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponseDto<object>>> ClearWaitlist(Guid courseId)
    {
        _logger.LogInformation("Clearing waitlist for course {CourseId}", courseId);

        await _waitlistService.ClearWaitlistAsync(courseId);

        return Ok(new ApiResponseDto<object>
        {
            Success = true,
            Message = "Waitlist cleared successfully"
        });
    }

    /// <summary>
    /// Notifies the next student on the waitlist (admin/system function)
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>Success status</returns>
    [HttpPost("course/{courseId}/notify-next")]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponseDto<object>>> NotifyNextStudent(Guid courseId)
    {
        _logger.LogInformation("Notifying next student on waitlist for course {CourseId}", courseId);

        await _waitlistService.NotifyNextStudentAsync(courseId);

        return Ok(new ApiResponseDto<object>
        {
            Success = true,
            Message = "Next student notified successfully"
        });
    }

    /// <summary>
    /// Reorders waitlist entries for a course (admin function)
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <param name="newPositions">Dictionary of waitlist entry IDs and their new positions</param>
    /// <returns>Success status</returns>
    [HttpPut("course/{courseId}/reorder")]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponseDto<object>>> ReorderWaitlist(
        Guid courseId,
        [FromBody] Dictionary<Guid, int> newPositions)
    {
        _logger.LogInformation("Reordering waitlist for course {CourseId}", courseId);

        try
        {
            await _waitlistService.ReorderWaitlistAsync(courseId, newPositions);

            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Waitlist reordered successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Failed to reorder waitlist: {Message}", ex.Message);
            return BadRequest(new ApiResponseDto<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }
}
