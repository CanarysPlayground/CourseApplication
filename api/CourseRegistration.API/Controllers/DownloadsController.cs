using Microsoft.AspNetCore.Mvc;
using CourseRegistration.Application.Interfaces;
using System.Text;

namespace CourseRegistration.API.Controllers;

/// <summary>
/// Controller for downloading course and registration data as CSV files
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DownloadsController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IRegistrationService _registrationService;
    private readonly ILogger<DownloadsController> _logger;

    /// <summary>
    /// Initializes a new instance of the DownloadsController
    /// </summary>
    public DownloadsController(
        ICourseService courseService,
        IRegistrationService registrationService,
        ILogger<DownloadsController> logger)
    {
        _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
        _registrationService = registrationService ?? throw new ArgumentNullException(nameof(registrationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Downloads all courses as a CSV file
    /// </summary>
    /// <returns>CSV file containing all courses</returns>
    [HttpGet("courses")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DownloadCourses()
    {
        _logger.LogInformation("Downloading all courses as CSV");

        var courses = await _courseService.SearchCoursesAsync(null, null);

        var csv = new StringBuilder();
        csv.AppendLine("CourseId,CourseName,InstructorName,Description,StartDate,EndDate,Schedule,CurrentEnrollment,IsActive");

        foreach (var course in courses)
        {
            csv.AppendLine(string.Join(",",
                EscapeCsvField(course.CourseId.ToString()),
                EscapeCsvField(course.CourseName),
                EscapeCsvField(course.InstructorName),
                EscapeCsvField(course.Description ?? string.Empty),
                EscapeCsvField(course.StartDate.ToString("yyyy-MM-dd")),
                EscapeCsvField(course.EndDate.ToString("yyyy-MM-dd")),
                EscapeCsvField(course.Schedule),
                EscapeCsvField(course.CurrentEnrollment.ToString()),
                EscapeCsvField(course.IsActive.ToString())));
        }

        var fileName = $"courses_{DateTime.UtcNow:yyyyMMdd}.csv";
        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", fileName);
    }

    /// <summary>
    /// Downloads all registrations as a CSV file
    /// </summary>
    /// <returns>CSV file containing all registrations</returns>
    [HttpGet("registrations")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DownloadRegistrations()
    {
        _logger.LogInformation("Downloading all registrations as CSV");

        var registrations = await _registrationService.GetAllRegistrationsAsync();

        var csv = new StringBuilder();
        csv.AppendLine("RegistrationId,StudentId,StudentName,StudentEmail,CourseId,CourseName,InstructorName,RegistrationDate,Status,Grade,Notes");

        foreach (var reg in registrations)
        {
            csv.AppendLine(string.Join(",",
                EscapeCsvField(reg.RegistrationId.ToString()),
                EscapeCsvField(reg.StudentId.ToString()),
                EscapeCsvField(reg.Student?.FullName ?? string.Empty),
                EscapeCsvField(reg.Student?.Email ?? string.Empty),
                EscapeCsvField(reg.CourseId.ToString()),
                EscapeCsvField(reg.Course?.CourseName ?? string.Empty),
                EscapeCsvField(reg.Course?.InstructorName ?? string.Empty),
                EscapeCsvField(reg.RegistrationDate.ToString("yyyy-MM-dd")),
                EscapeCsvField(reg.Status.ToString()),
                EscapeCsvField(reg.Grade?.ToString() ?? string.Empty),
                EscapeCsvField(reg.Notes ?? string.Empty)));
        }

        var fileName = $"registrations_{DateTime.UtcNow:yyyyMMdd}.csv";
        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv", fileName);
    }

    /// <summary>
    /// Escapes a CSV field value to prevent injection and handle special characters.
    /// Fields containing commas, double quotes, or newlines are wrapped in double quotes.
    /// Double quotes within the value are escaped by doubling them.
    /// Leading = + - @ characters are prefixed with a tab to prevent formula injection.
    /// The tab prefix is applied before quoting so it is preserved correctly in all cases.
    /// </summary>
    private static string EscapeCsvField(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        // Prevent CSV formula injection by neutralizing dangerous leading characters.
        // This covers the primary injection vectors: = (formula), + (formula), - (formula/unary),
        // @ (DDE/macro trigger). Tab-prefix is the recommended approach as it is invisible to end
        // users while preventing spreadsheet apps from interpreting the cell as a formula.
        if (value[0] is '=' or '+' or '-' or '@')
        {
            value = "\t" + value;
        }

        // Quote the field if it contains characters that need escaping.
        // This is applied after the optional tab-prefix, so the tab is preserved inside quotes.
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        return value;
    }
}
