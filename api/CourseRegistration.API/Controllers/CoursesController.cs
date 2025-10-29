using Microsoft.AspNetCore.Mvc;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Interfaces;

namespace CourseRegistration.API.Controllers;

/// <summary>
/// Controller for course management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ILogger<CoursesController> _logger;

    /// <summary>
    /// Initializes a new instance of the CoursesController
    /// </summary>
    public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
    {
        _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all courses with pagination and filtering
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for course name or description</param>
    /// <param name="instructor">Filter by instructor name</param>
    /// <returns>Paginated list of courses</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<PagedResponseDto<CourseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponseDto<PagedResponseDto<CourseDto>>>> GetCourses(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? instructor = null)
    {
        _logger.LogInformation("Getting courses with page: {Page}, pageSize: {PageSize}, searchTerm: {SearchTerm}, instructor: {Instructor}", 
            page, pageSize, searchTerm, instructor);
        
        var result = await _courseService.GetCoursesAsync(page, pageSize, searchTerm, instructor);
        return Ok(ApiResponseDto<PagedResponseDto<CourseDto>>.SuccessResponse(result, "Courses retrieved successfully"));
    }

    /// <summary>
    /// Gets a course by ID with enrollment details
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>Course details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponseDto<CourseDto>>> GetCourse(Guid id)
    {
        _logger.LogInformation("Getting course with ID: {CourseId}", id);
        
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
        {
            return NotFound(ApiResponseDto<CourseDto>.ErrorResponse("Course not found"));
        }

        return Ok(ApiResponseDto<CourseDto>.SuccessResponse(course, "Course retrieved successfully"));
    }

    /// <summary>
    /// Creates a new course
    /// </summary>
    /// <param name="createCourseDto">Course creation details</param>
    /// <returns>Created course</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<CourseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponseDto<CourseDto>>> CreateCourse([FromBody] CreateCourseDto createCourseDto)
    {
        _logger.LogInformation("Creating new course: {CourseName}", createCourseDto.CourseName);
        
        var course = await _courseService.CreateCourseAsync(createCourseDto);
        
        return CreatedAtAction(
            nameof(GetCourse),
            new { id = course.CourseId },
            ApiResponseDto<CourseDto>.SuccessResponse(course, "Course created successfully"));
    }

    /// <summary>
    /// Updates an existing course
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <param name="updateCourseDto">Course update details</param>
    /// <returns>Updated course</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponseDto<CourseDto>>> UpdateCourse(Guid id, [FromBody] UpdateCourseDto updateCourseDto)
    {
        _logger.LogInformation("Updating course with ID: {CourseId}", id);
        
        var course = await _courseService.UpdateCourseAsync(id, updateCourseDto);
        if (course == null)
        {
            return NotFound(ApiResponseDto<CourseDto>.ErrorResponse("Course not found"));
        }

        return Ok(ApiResponseDto<CourseDto>.SuccessResponse(course, "Course updated successfully"));
    }

    /// <summary>
    /// Deletes a course (soft delete)
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponseDto<object>>> DeleteCourse(Guid id)
    {
        _logger.LogInformation("Deleting course with ID: {CourseId}", id);
        
        var result = await _courseService.DeleteCourseAsync(id);
        if (!result)
        {
            return NotFound(ApiResponseDto<object>.ErrorResponse("Course not found"));
        }

        return Ok(ApiResponseDto<object>.SuccessResponse(null, "Course deleted successfully"));
    }

    /// <summary>
    /// Searches courses by name, instructor, or description
    /// </summary>
    /// <param name="searchTerm">Search term for course content</param>
    /// <param name="instructor">Filter by instructor name</param>
    /// <returns>List of matching courses</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<CourseDto>>>> SearchCourses(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? instructor = null)
    {
        _logger.LogInformation("Searching courses with term: {SearchTerm}, instructor: {Instructor}", searchTerm, instructor);
        
        var courses = await _courseService.SearchCoursesAsync(searchTerm, instructor);
        return Ok(ApiResponseDto<IEnumerable<CourseDto>>.SuccessResponse(courses, "Course search completed"));
    }

    /// <summary>
    /// Gets available courses for registration
    /// </summary>
    /// <returns>List of available courses</returns>
    [HttpGet("available")]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<CourseDto>>>> GetAvailableCourses()
    {
        _logger.LogInformation("Getting available courses for registration");
        
        var courses = await _courseService.GetAvailableCoursesAsync();
        return Ok(ApiResponseDto<IEnumerable<CourseDto>>.SuccessResponse(courses, "Available courses retrieved successfully"));
    }

    /// <summary>
    /// Gets courses by instructor
    /// </summary>
    /// <param name="instructorName">Instructor name</param>
    /// <returns>List of courses by the instructor</returns>
    [HttpGet("instructor/{instructorName}")]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<CourseDto>>>> GetCoursesByInstructor(string instructorName)
    {
        _logger.LogInformation("Getting courses for instructor: {InstructorName}", instructorName);
        
        var courses = await _courseService.GetCoursesByInstructorAsync(instructorName);
        return Ok(ApiResponseDto<IEnumerable<CourseDto>>.SuccessResponse(courses, $"Courses for instructor {instructorName} retrieved successfully"));
    }

    /// <summary>
    /// Gets a course's registrations
    /// </summary>
    /// <param name="id">Course ID</param>
    /// <returns>List of course registrations</returns>
    [HttpGet("{id:guid}/registrations")]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<RegistrationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<RegistrationDto>>>> GetCourseRegistrations(Guid id)
    {
        _logger.LogInformation("Getting registrations for course ID: {CourseId}", id);
        
        // First check if course exists
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
        {
            return NotFound(ApiResponseDto<IEnumerable<RegistrationDto>>.ErrorResponse("Course not found"));
        }

        var registrations = await _courseService.GetCourseRegistrationsAsync(id);
        return Ok(ApiResponseDto<IEnumerable<RegistrationDto>>.SuccessResponse(registrations, "Course registrations retrieved successfully"));
    }
}