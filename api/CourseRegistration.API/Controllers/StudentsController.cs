using Microsoft.AspNetCore.Mvc;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Interfaces;

namespace CourseRegistration.API.Controllers;

/// <summary>
/// Controller for student management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ILogger<StudentsController> _logger;

    /// <summary>
    /// Initializes a new instance of the StudentsController
    /// </summary>
    public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
    {
        _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all students with pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <returns>Paginated list of students</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<PagedResponseDto<StudentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponseDto<PagedResponseDto<StudentDto>>>> GetStudents(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Getting students with page: {Page}, pageSize: {PageSize}", page, pageSize);
        
        var result = await _studentService.GetStudentsAsync(page, pageSize);
        return Ok(ApiResponseDto<PagedResponseDto<StudentDto>>.SuccessResponse(result, "Students retrieved successfully"));
    }

    /// <summary>
    /// Gets a student by ID
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <returns>Student details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponseDto<StudentDto>>> GetStudent(Guid id)
    {
        _logger.LogInformation("Getting student with ID: {StudentId}", id);
        
        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null)
        {
            return NotFound(ApiResponseDto<StudentDto>.ErrorResponse("Student not found"));
        }

        return Ok(ApiResponseDto<StudentDto>.SuccessResponse(student, "Student retrieved successfully"));
    }

    /// <summary>
    /// Creates a new student
    /// </summary>
    /// <param name="createStudentDto">Student creation details</param>
    /// <returns>Created student</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<StudentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponseDto<StudentDto>>> CreateStudent([FromBody] CreateStudentDto createStudentDto)
    {
        _logger.LogInformation("Creating new student with email: {Email}", createStudentDto.Email);
        
        var student = await _studentService.CreateStudentAsync(createStudentDto);
        
        return CreatedAtAction(
            nameof(GetStudent), 
            new { id = student.StudentId }, 
            ApiResponseDto<StudentDto>.SuccessResponse(student, "Student created successfully"));
    }

    /// <summary>
    /// Updates an existing student
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <param name="updateStudentDto">Student update details</param>
    /// <returns>Updated student</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponseDto<StudentDto>>> UpdateStudent(Guid id, [FromBody] UpdateStudentDto updateStudentDto)
    {
        _logger.LogInformation("Updating student with ID: {StudentId}", id);
        
        var student = await _studentService.UpdateStudentAsync(id, updateStudentDto);
        if (student == null)
        {
            return NotFound(ApiResponseDto<StudentDto>.ErrorResponse("Student not found"));
        }

        return Ok(ApiResponseDto<StudentDto>.SuccessResponse(student, "Student updated successfully"));
    }

    /// <summary>
    /// Deletes a student (soft delete)
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponseDto<object>>> DeleteStudent(Guid id)
    {
        _logger.LogInformation("Deleting student with ID: {StudentId}", id);
        
        var result = await _studentService.DeleteStudentAsync(id);
        if (!result)
        {
            return NotFound(ApiResponseDto<object>.ErrorResponse("Student not found"));
        }

        return Ok(ApiResponseDto<object>.SuccessResponse(null, "Student deleted successfully"));
    }

    /// <summary>
    /// Searches students by name
    /// </summary>
    /// <param name="searchTerm">Search term for student name or email</param>
    /// <returns>List of matching students</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<StudentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<StudentDto>>>> SearchStudents(
        [FromQuery] string searchTerm = "")
    {
        _logger.LogInformation("Searching students with term: {SearchTerm}", searchTerm);
        
        var students = await _studentService.SearchStudentsAsync(searchTerm);
        return Ok(ApiResponseDto<IEnumerable<StudentDto>>.SuccessResponse(students, "Student search completed"));
    }

    /// <summary>
    /// Gets a student's registrations
    /// </summary>
    /// <param name="id">Student ID</param>
    /// <returns>List of student's registrations</returns>
    [HttpGet("{id:guid}/registrations")]
    [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<RegistrationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<RegistrationDto>>>> GetStudentRegistrations(Guid id)
    {
        _logger.LogInformation("Getting registrations for student ID: {StudentId}", id);
        
        // First check if student exists
        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null)
        {
            return NotFound(ApiResponseDto<IEnumerable<RegistrationDto>>.ErrorResponse("Student not found"));
        }

        var registrations = await _studentService.GetStudentRegistrationsAsync(id);
        return Ok(ApiResponseDto<IEnumerable<RegistrationDto>>.SuccessResponse(registrations, "Student registrations retrieved successfully"));
    }
}