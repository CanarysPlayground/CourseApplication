using Microsoft.AspNetCore.Mvc;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Interfaces;

namespace CourseRegistration.API.Controllers;

/// <summary>
/// Controller for instructor rating operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InstructorRatingsController : ControllerBase
{
    private readonly IInstructorRatingService _ratingService;
    private readonly ILogger<InstructorRatingsController> _logger;

    /// <summary>
    /// Initializes a new instance of the InstructorRatingsController
    /// </summary>
    public InstructorRatingsController(IInstructorRatingService ratingService, ILogger<InstructorRatingsController> logger)
    {
        _ratingService = ratingService ?? throw new ArgumentNullException(nameof(ratingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new instructor rating
    /// </summary>
    /// <param name="createRatingDto">Rating creation details</param>
    /// <returns>Created rating</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<InstructorRatingDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponseDto<InstructorRatingDto>>> CreateRating([FromBody] CreateInstructorRatingDto createRatingDto)
    {
        _logger.LogInformation("Creating new rating for course {CourseId} by student {StudentId}", 
            createRatingDto.CourseId, createRatingDto.StudentId);

        try
        {
            var rating = await _ratingService.CreateRatingAsync(createRatingDto);
            
            return CreatedAtAction(
                nameof(GetRating),
                new { id = rating.RatingId },
                ApiResponseDto<InstructorRatingDto>.SuccessResponse(rating, "Rating created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Failed to create rating: {Message}", ex.Message);
            return BadRequest(ApiResponseDto<InstructorRatingDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Gets a rating by ID
    /// </summary>
    /// <param name="id">Rating ID</param>
    /// <returns>Rating details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<InstructorRatingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponseDto<InstructorRatingDto>>> GetRating(Guid id)
    {
        _logger.LogInformation("Getting rating with ID: {RatingId}", id);
        
        var rating = await _ratingService.GetRatingByIdAsync(id);
        if (rating == null)
        {
            return NotFound(ApiResponseDto<InstructorRatingDto>.ErrorResponse("Rating not found"));
        }

        return Ok(ApiResponseDto<InstructorRatingDto>.SuccessResponse(rating, "Rating retrieved successfully"));
    }

    /// <summary>
    /// Gets all ratings for a specific course
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>List of ratings</returns>
    [HttpGet("course/{courseId:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<InstructorRatingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<InstructorRatingDto>>>> GetRatingsByCourse(Guid courseId)
    {
        _logger.LogInformation("Getting ratings for course: {CourseId}", courseId);
        
        var ratings = await _ratingService.GetRatingsByCourseIdAsync(courseId);
        return Ok(ApiResponseDto<IEnumerable<InstructorRatingDto>>.SuccessResponse(ratings, "Ratings retrieved successfully"));
    }

    /// <summary>
    /// Gets all ratings submitted by a specific student
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <returns>List of ratings</returns>
    [HttpGet("student/{studentId:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<InstructorRatingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<InstructorRatingDto>>>> GetRatingsByStudent(Guid studentId)
    {
        _logger.LogInformation("Getting ratings by student: {StudentId}", studentId);
        
        var ratings = await _ratingService.GetRatingsByStudentIdAsync(studentId);
        return Ok(ApiResponseDto<IEnumerable<InstructorRatingDto>>.SuccessResponse(ratings, "Ratings retrieved successfully"));
    }

    /// <summary>
    /// Gets all ratings for a specific instructor
    /// </summary>
    /// <param name="instructorName">Instructor name</param>
    /// <returns>List of ratings</returns>
    [HttpGet("instructor/{instructorName}")]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<InstructorRatingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<InstructorRatingDto>>>> GetRatingsByInstructor(string instructorName)
    {
        _logger.LogInformation("Getting ratings for instructor: {InstructorName}", instructorName);
        
        var ratings = await _ratingService.GetRatingsByInstructorNameAsync(instructorName);
        return Ok(ApiResponseDto<IEnumerable<InstructorRatingDto>>.SuccessResponse(ratings, "Ratings retrieved successfully"));
    }

    /// <summary>
    /// Gets rating statistics for an instructor
    /// </summary>
    /// <param name="instructorName">Instructor name</param>
    /// <returns>Rating statistics</returns>
    [HttpGet("instructor/{instructorName}/stats")]
    [ProducesResponseType(typeof(ApiResponseDto<InstructorRatingStatsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponseDto<InstructorRatingStatsDto>>> GetInstructorStats(string instructorName)
    {
        _logger.LogInformation("Getting stats for instructor: {InstructorName}", instructorName);
        
        var stats = await _ratingService.GetInstructorStatsAsync(instructorName);
        if (stats == null)
        {
            return NotFound(ApiResponseDto<InstructorRatingStatsDto>.ErrorResponse("No ratings found for this instructor"));
        }

        return Ok(ApiResponseDto<InstructorRatingStatsDto>.SuccessResponse(stats, "Statistics retrieved successfully"));
    }

    /// <summary>
    /// Gets a rating by student and course
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="courseId">Course ID</param>
    /// <returns>Rating details</returns>
    [HttpGet("student/{studentId:guid}/course/{courseId:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<InstructorRatingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponseDto<InstructorRatingDto>>> GetRatingByStudentAndCourse(Guid studentId, Guid courseId)
    {
        _logger.LogInformation("Getting rating for student {StudentId} and course {CourseId}", studentId, courseId);
        
        var rating = await _ratingService.GetRatingByStudentAndCourseAsync(studentId, courseId);
        if (rating == null)
        {
            return NotFound(ApiResponseDto<InstructorRatingDto>.ErrorResponse("Rating not found"));
        }

        return Ok(ApiResponseDto<InstructorRatingDto>.SuccessResponse(rating, "Rating retrieved successfully"));
    }

    /// <summary>
    /// Updates an existing rating
    /// </summary>
    /// <param name="id">Rating ID</param>
    /// <param name="updateRatingDto">Rating update details</param>
    /// <returns>Updated rating</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<InstructorRatingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponseDto<InstructorRatingDto>>> UpdateRating(Guid id, [FromBody] UpdateInstructorRatingDto updateRatingDto)
    {
        _logger.LogInformation("Updating rating with ID: {RatingId}", id);
        
        var rating = await _ratingService.UpdateRatingAsync(id, updateRatingDto);
        if (rating == null)
        {
            return NotFound(ApiResponseDto<InstructorRatingDto>.ErrorResponse("Rating not found"));
        }

        return Ok(ApiResponseDto<InstructorRatingDto>.SuccessResponse(rating, "Rating updated successfully"));
    }

    /// <summary>
    /// Deletes a rating
    /// </summary>
    /// <param name="id">Rating ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponseDto<object>>> DeleteRating(Guid id)
    {
        _logger.LogInformation("Deleting rating with ID: {RatingId}", id);
        
        var success = await _ratingService.DeleteRatingAsync(id);
        if (!success)
        {
            return NotFound(ApiResponseDto<object>.ErrorResponse("Rating not found"));
        }

        return Ok(ApiResponseDto<object>.SuccessResponse(null, "Rating deleted successfully"));
    }
}
