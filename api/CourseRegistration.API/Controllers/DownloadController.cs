using Microsoft.AspNetCore.Mvc;
using CourseRegistration.Application.Interfaces;
using System.Text;

namespace CourseRegistration.API.Controllers;

/// <summary>
/// Controller for downloading course, student, and registration data as CSV files
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DownloadController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IStudentService _studentService;
    private readonly IRegistrationService _registrationService;
    private readonly ILogger<DownloadController> _logger;

    /// <summary>
    /// Initializes a new instance of the DownloadController
    /// </summary>
    public DownloadController(
        ICourseService courseService,
        IStudentService studentService,
        IRegistrationService registrationService,
        ILogger<DownloadController> logger)
    {
        _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
        _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
        _registrationService = registrationService ?? throw new ArgumentNullException(nameof(registrationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Downloads all courses as a CSV file
    /// </summary>
    /// <returns>CSV file containing course data</returns>
    [HttpGet("courses")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> DownloadCourses()
    {
        _logger.LogInformation("Downloading courses as CSV");

        var result = await _courseService.GetCoursesAsync(page: 1, pageSize: int.MaxValue);
        var courses = result.Items;

        var csv = new StringBuilder();
        csv.AppendLine("CourseId,CourseName,Description,InstructorName,StartDate,EndDate,Schedule,CurrentEnrollment,IsActive,CreatedAt");

        foreach (var course in courses)
        {
            csv.AppendLine(string.Join(",",
                course.CourseId,
                EscapeCsvField(course.CourseName),
                EscapeCsvField(course.Description ?? string.Empty),
                EscapeCsvField(course.InstructorName),
                course.StartDate.ToString("yyyy-MM-dd"),
                course.EndDate.ToString("yyyy-MM-dd"),
                EscapeCsvField(course.Schedule),
                course.CurrentEnrollment,
                course.IsActive,
                course.CreatedAt.ToString("yyyy-MM-dd")));
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", $"courses_{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    /// <summary>
    /// Downloads all students as a CSV file
    /// </summary>
    /// <returns>CSV file containing student data</returns>
    [HttpGet("students")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> DownloadStudents()
    {
        _logger.LogInformation("Downloading students as CSV");

        var result = await _studentService.GetStudentsAsync(page: 1, pageSize: int.MaxValue);
        var students = result.Items;

        var csv = new StringBuilder();
        csv.AppendLine("StudentId,FirstName,LastName,Email,PhoneNumber,DateOfBirth,CreatedAt");

        foreach (var student in students)
        {
            csv.AppendLine(string.Join(",",
                student.StudentId,
                EscapeCsvField(student.FirstName),
                EscapeCsvField(student.LastName),
                EscapeCsvField(student.Email),
                EscapeCsvField(student.PhoneNumber ?? string.Empty),
                student.DateOfBirth.ToString("yyyy-MM-dd"),
                student.CreatedAt.ToString("yyyy-MM-dd")));
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", $"students_{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    /// <summary>
    /// Downloads all registrations as a CSV file
    /// </summary>
    /// <returns>CSV file containing registration data</returns>
    [HttpGet("registrations")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> DownloadRegistrations()
    {
        _logger.LogInformation("Downloading registrations as CSV");

        var result = await _registrationService.GetRegistrationsAsync(page: 1, pageSize: int.MaxValue);
        var registrations = result.Items;

        var csv = new StringBuilder();
        csv.AppendLine("RegistrationId,StudentId,StudentName,CourseId,CourseName,RegistrationDate,Status,Grade,Notes");

        foreach (var reg in registrations)
        {
            csv.AppendLine(string.Join(",",
                reg.RegistrationId,
                reg.StudentId,
                EscapeCsvField(reg.Student != null ? $"{reg.Student.FirstName} {reg.Student.LastName}" : string.Empty),
                reg.CourseId,
                EscapeCsvField(reg.Course?.CourseName ?? string.Empty),
                reg.RegistrationDate.ToString("yyyy-MM-dd"),
                reg.Status,
                reg.Grade?.ToString() ?? string.Empty,
                EscapeCsvField(reg.Notes ?? string.Empty)));
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", $"registrations_{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    private static string EscapeCsvField(string field)
    {
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }
}
