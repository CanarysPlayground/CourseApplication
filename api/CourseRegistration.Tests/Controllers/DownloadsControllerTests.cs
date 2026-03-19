using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using CourseRegistration.API.Controllers;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Interfaces;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Tests.Controllers;

/// <summary>
/// Unit tests for the DownloadsController
/// </summary>
public class DownloadsControllerTests
{
    // -------------------------------------------------------------------------
    // Hand-written test fakes
    // -------------------------------------------------------------------------

    private sealed class FakeCourseService : ICourseService
    {
        private readonly IEnumerable<CourseDto> _courses;

        public FakeCourseService(IEnumerable<CourseDto> courses)
        {
            _courses = courses;
        }

        public Task<IEnumerable<CourseDto>> SearchCoursesAsync(string? searchTerm, string? instructor)
            => Task.FromResult(_courses);

        // Unused members
        public Task<PagedResponseDto<CourseDto>> GetCoursesAsync(int page, int pageSize, string? searchTerm, string? instructor) => throw new NotImplementedException();
        public Task<CourseDto?> GetCourseByIdAsync(Guid id) => throw new NotImplementedException();
        public Task<CourseDto> CreateCourseAsync(CreateCourseDto dto) => throw new NotImplementedException();
        public Task<CourseDto?> UpdateCourseAsync(Guid id, UpdateCourseDto dto) => throw new NotImplementedException();
        public Task<bool> DeleteCourseAsync(Guid id) => throw new NotImplementedException();
        public Task<IEnumerable<CourseDto>> GetAvailableCoursesAsync() => throw new NotImplementedException();
        public Task<IEnumerable<CourseDto>> GetCoursesByInstructorAsync(string instructorName) => throw new NotImplementedException();
        public Task<IEnumerable<RegistrationDto>> GetCourseRegistrationsAsync(Guid courseId) => throw new NotImplementedException();
    }

    private sealed class FakeRegistrationService : IRegistrationService
    {
        private readonly IEnumerable<RegistrationDto> _registrations;

        public FakeRegistrationService(IEnumerable<RegistrationDto> registrations)
        {
            _registrations = registrations;
        }

        public Task<IEnumerable<RegistrationDto>> GetAllRegistrationsAsync()
            => Task.FromResult(_registrations);

        // Unused members
        public Task<PagedResponseDto<RegistrationDto>> GetRegistrationsAsync(int page, int pageSize, Guid? studentId, Guid? courseId, RegistrationStatus? status) => throw new NotImplementedException();
        public Task<RegistrationDto?> GetRegistrationByIdAsync(Guid id) => throw new NotImplementedException();
        public Task<RegistrationDto> CreateRegistrationAsync(CreateRegistrationDto dto) => throw new NotImplementedException();
        public Task<RegistrationDto?> UpdateRegistrationStatusAsync(Guid id, UpdateRegistrationStatusDto dto) => throw new NotImplementedException();
        public Task<bool> CancelRegistrationAsync(Guid id) => throw new NotImplementedException();
        public Task<IEnumerable<RegistrationDto>> GetRegistrationsByStudentAsync(Guid studentId) => throw new NotImplementedException();
        public Task<IEnumerable<RegistrationDto>> GetRegistrationsByCourseAsync(Guid courseId) => throw new NotImplementedException();
        public Task<IEnumerable<RegistrationDto>> GetRegistrationsByStatusAsync(RegistrationStatus status) => throw new NotImplementedException();
        public Task<bool> IsStudentRegisteredForCourseAsync(Guid studentId, Guid courseId) => throw new NotImplementedException();
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static DownloadsController CreateController(
        IEnumerable<CourseDto>? courses = null,
        IEnumerable<RegistrationDto>? registrations = null)
    {
        var courseService = new FakeCourseService(courses ?? Enumerable.Empty<CourseDto>());
        var registrationService = new FakeRegistrationService(registrations ?? Enumerable.Empty<RegistrationDto>());
        return new DownloadsController(courseService, registrationService, NullLogger<DownloadsController>.Instance);
    }

    private static string ReadFileResult(IActionResult result)
    {
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);
        return System.Text.Encoding.UTF8.GetString(fileResult.FileContents);
    }

    // -------------------------------------------------------------------------
    // DownloadCourses tests
    // -------------------------------------------------------------------------

    [Fact]
    public async Task DownloadCourses_EmptyList_ReturnsCsvWithHeaderOnly()
    {
        // Arrange
        var controller = CreateController(courses: Enumerable.Empty<CourseDto>());

        // Act
        var result = await controller.DownloadCourses();

        // Assert
        var csv = ReadFileResult(result);
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.Single(lines); // only header
        Assert.Contains("CourseName", lines[0]);
        Assert.Contains("InstructorName", lines[0]);
    }

    [Fact]
    public async Task DownloadCourses_WithCourses_ReturnsCsvWithHeaderAndDataRows()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var courses = new[]
        {
            new CourseDto
            {
                CourseId = courseId,
                CourseName = "Introduction to C#",
                InstructorName = "Dr. Smith",
                Description = "A beginner course",
                StartDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc),
                Schedule = "MWF 9:00-10:30 AM",
                CurrentEnrollment = 5,
                IsActive = true
            }
        };
        var controller = CreateController(courses: courses);

        // Act
        var result = await controller.DownloadCourses();

        // Assert
        var csv = ReadFileResult(result);
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length); // header + 1 data row
        Assert.Contains("Introduction to C#", lines[1]);
        Assert.Contains("Dr. Smith", lines[1]);
        Assert.Contains(courseId.ToString(), lines[1]);
    }

    [Fact]
    public async Task DownloadCourses_FieldWithComma_IsQuotedInCsv()
    {
        // Arrange
        var courses = new[]
        {
            new CourseDto
            {
                CourseId = Guid.NewGuid(),
                CourseName = "Science, Technology, Engineering",
                InstructorName = "Dr. Jones",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddMonths(6),
                Schedule = "TTh 2:00-3:30 PM"
            }
        };
        var controller = CreateController(courses: courses);

        // Act
        var result = await controller.DownloadCourses();

        // Assert
        var csv = ReadFileResult(result);
        Assert.Contains("\"Science, Technology, Engineering\"", csv);
    }

    [Fact]
    public async Task DownloadCourses_ReturnsFileWithCsvContentType()
    {
        // Arrange
        var controller = CreateController();

        // Act
        var result = await controller.DownloadCourses();

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);
        Assert.StartsWith("courses_", fileResult.FileDownloadName);
        Assert.EndsWith(".csv", fileResult.FileDownloadName);
    }

    // -------------------------------------------------------------------------
    // DownloadRegistrations tests
    // -------------------------------------------------------------------------

    [Fact]
    public async Task DownloadRegistrations_EmptyList_ReturnsCsvWithHeaderOnly()
    {
        // Arrange
        var controller = CreateController(registrations: Enumerable.Empty<RegistrationDto>());

        // Act
        var result = await controller.DownloadRegistrations();

        // Assert
        var csv = ReadFileResult(result);
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.Single(lines); // only header
        Assert.Contains("RegistrationId", lines[0]);
        Assert.Contains("StudentName", lines[0]);
        Assert.Contains("CourseName", lines[0]);
    }

    [Fact]
    public async Task DownloadRegistrations_WithRegistrations_ReturnsCsvWithHeaderAndDataRows()
    {
        // Arrange
        var registrationId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var registrations = new[]
        {
            new RegistrationDto
            {
                RegistrationId = registrationId,
                StudentId = studentId,
                CourseId = courseId,
                Student = new StudentDto { FullName = "Jane Doe", Email = "jane@example.com" },
                Course = new CourseDto { CourseName = "Advanced Math", InstructorName = "Prof. Brown" },
                RegistrationDate = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                Status = RegistrationStatus.Confirmed,
                Notes = "Priority student"
            }
        };
        var controller = CreateController(registrations: registrations);

        // Act
        var result = await controller.DownloadRegistrations();

        // Assert
        var csv = ReadFileResult(result);
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length); // header + 1 data row
        Assert.Contains("Jane Doe", lines[1]);
        Assert.Contains("Advanced Math", lines[1]);
        Assert.Contains(registrationId.ToString(), lines[1]);
        Assert.Contains("Confirmed", lines[1]);
    }

    [Fact]
    public async Task DownloadRegistrations_ReturnsFileWithCsvContentType()
    {
        // Arrange
        var controller = CreateController();

        // Act
        var result = await controller.DownloadRegistrations();

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);
        Assert.StartsWith("registrations_", fileResult.FileDownloadName);
        Assert.EndsWith(".csv", fileResult.FileDownloadName);
    }

    [Fact]
    public async Task DownloadRegistrations_FieldWithDoubleQuote_IsEscapedInCsv()
    {
        // Arrange
        var registrations = new[]
        {
            new RegistrationDto
            {
                RegistrationId = Guid.NewGuid(),
                StudentId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                RegistrationDate = DateTime.UtcNow,
                Status = RegistrationStatus.Pending,
                Notes = "She said \"hello\""
            }
        };
        var controller = CreateController(registrations: registrations);

        // Act
        var result = await controller.DownloadRegistrations();

        // Assert
        var csv = ReadFileResult(result);
        // Double quotes are escaped by doubling: "She said ""hello"""
        Assert.Contains("She said \"\"hello\"\"", csv);
    }

    [Fact]
    public async Task DownloadCourses_FormulaInjectionField_IsPrefixedWithTab()
    {
        // Arrange
        var courses = new[]
        {
            new CourseDto
            {
                CourseId = Guid.NewGuid(),
                CourseName = "=SUM(1+1)",
                InstructorName = "Dr. Evil",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddMonths(6),
                Schedule = "MWF"
            }
        };
        var controller = CreateController(courses: courses);

        // Act
        var result = await controller.DownloadCourses();

        // Assert
        var csv = ReadFileResult(result);
        // The formula-prefixed field should be tab-prefixed to neutralize injection
        Assert.Contains("\t=SUM(1+1)", csv);
    }
}
