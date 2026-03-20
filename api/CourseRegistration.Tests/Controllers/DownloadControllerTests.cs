using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CourseRegistration.API.Controllers;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Interfaces;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Tests.Controllers;

/// <summary>
/// Unit tests for DownloadController CSV download endpoints
/// </summary>
public class DownloadControllerTests
{
    private readonly Mock<ICourseService> _mockCourseService;
    private readonly Mock<IStudentService> _mockStudentService;
    private readonly Mock<IRegistrationService> _mockRegistrationService;
    private readonly Mock<ILogger<DownloadController>> _mockLogger;
    private readonly DownloadController _controller;

    public DownloadControllerTests()
    {
        _mockCourseService = new Mock<ICourseService>();
        _mockStudentService = new Mock<IStudentService>();
        _mockRegistrationService = new Mock<IRegistrationService>();
        _mockLogger = new Mock<ILogger<DownloadController>>();

        _controller = new DownloadController(
            _mockCourseService.Object,
            _mockStudentService.Object,
            _mockRegistrationService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task DownloadCourses_ReturnsFileResult_WithCsvContentType()
    {
        // Arrange
        var courses = new PagedResponseDto<CourseDto>
        {
            Items = new List<CourseDto>
            {
                new CourseDto
                {
                    CourseId = Guid.NewGuid(),
                    CourseName = "Introduction to CS",
                    Description = "Basics of CS",
                    InstructorName = "Dr. Smith",
                    StartDate = new DateTime(2024, 1, 15),
                    EndDate = new DateTime(2024, 5, 15),
                    Schedule = "MWF 9:00-10:30",
                    CurrentEnrollment = 25,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1)
                }
            },
            TotalItems = 1
        };

        _mockCourseService
            .Setup(s => s.GetCoursesAsync(1, int.MaxValue, null, null))
            .ReturnsAsync(courses);

        // Act
        var result = await _controller.DownloadCourses();

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);
        Assert.StartsWith("courses_", fileResult.FileDownloadName);
        Assert.EndsWith(".csv", fileResult.FileDownloadName);
    }

    [Fact]
    public async Task DownloadCourses_CsvContent_ContainsHeaderRow()
    {
        // Arrange
        var courses = new PagedResponseDto<CourseDto>
        {
            Items = Enumerable.Empty<CourseDto>(),
            TotalItems = 0
        };

        _mockCourseService
            .Setup(s => s.GetCoursesAsync(1, int.MaxValue, null, null))
            .ReturnsAsync(courses);

        // Act
        var result = await _controller.DownloadCourses();

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        var content = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);
        Assert.Contains("CourseId,CourseName", content);
        Assert.Contains("InstructorName", content);
    }

    [Fact]
    public async Task DownloadCourses_CsvContent_ContainsCourseData()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var courses = new PagedResponseDto<CourseDto>
        {
            Items = new List<CourseDto>
            {
                new CourseDto
                {
                    CourseId = courseId,
                    CourseName = "Web Development",
                    InstructorName = "Prof. Jones",
                    StartDate = new DateTime(2024, 2, 1),
                    EndDate = new DateTime(2024, 6, 1),
                    Schedule = "TTh 14:00-16:00",
                    CurrentEnrollment = 30,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 10)
                }
            },
            TotalItems = 1
        };

        _mockCourseService
            .Setup(s => s.GetCoursesAsync(1, int.MaxValue, null, null))
            .ReturnsAsync(courses);

        // Act
        var result = await _controller.DownloadCourses();

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        var content = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);
        Assert.Contains("Web Development", content);
        Assert.Contains("Prof. Jones", content);
        Assert.Contains(courseId.ToString(), content);
    }

    [Fact]
    public async Task DownloadStudents_ReturnsFileResult_WithCsvContentType()
    {
        // Arrange
        var students = new PagedResponseDto<StudentDto>
        {
            Items = new List<StudentDto>
            {
                new StudentDto
                {
                    StudentId = Guid.NewGuid(),
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "+1-555-0101",
                    DateOfBirth = new DateTime(1995, 5, 15),
                    CreatedAt = new DateTime(2024, 1, 1)
                }
            },
            TotalItems = 1
        };

        _mockStudentService
            .Setup(s => s.GetStudentsAsync(1, int.MaxValue))
            .ReturnsAsync(students);

        // Act
        var result = await _controller.DownloadStudents();

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);
        Assert.StartsWith("students_", fileResult.FileDownloadName);
        Assert.EndsWith(".csv", fileResult.FileDownloadName);
    }

    [Fact]
    public async Task DownloadStudents_CsvContent_ContainsHeaderRow()
    {
        // Arrange
        var students = new PagedResponseDto<StudentDto>
        {
            Items = Enumerable.Empty<StudentDto>(),
            TotalItems = 0
        };

        _mockStudentService
            .Setup(s => s.GetStudentsAsync(1, int.MaxValue))
            .ReturnsAsync(students);

        // Act
        var result = await _controller.DownloadStudents();

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        var content = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);
        Assert.Contains("StudentId,FirstName,LastName,Email", content);
        Assert.Contains("DateOfBirth", content);
    }

    [Fact]
    public async Task DownloadStudents_CsvContent_ContainsStudentData()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var students = new PagedResponseDto<StudentDto>
        {
            Items = new List<StudentDto>
            {
                new StudentDto
                {
                    StudentId = studentId,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    DateOfBirth = new DateTime(1996, 8, 20),
                    CreatedAt = new DateTime(2024, 1, 5)
                }
            },
            TotalItems = 1
        };

        _mockStudentService
            .Setup(s => s.GetStudentsAsync(1, int.MaxValue))
            .ReturnsAsync(students);

        // Act
        var result = await _controller.DownloadStudents();

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        var content = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);
        Assert.Contains("Jane", content);
        Assert.Contains("Smith", content);
        Assert.Contains("jane.smith@example.com", content);
        Assert.Contains(studentId.ToString(), content);
    }

    [Fact]
    public async Task DownloadRegistrations_ReturnsFileResult_WithCsvContentType()
    {
        // Arrange
        var registrations = new PagedResponseDto<RegistrationDto>
        {
            Items = new List<RegistrationDto>
            {
                new RegistrationDto
                {
                    RegistrationId = Guid.NewGuid(),
                    StudentId = Guid.NewGuid(),
                    CourseId = Guid.NewGuid(),
                    RegistrationDate = DateTime.UtcNow.AddDays(-5),
                    Status = RegistrationStatus.Confirmed,
                    Student = new StudentDto { FirstName = "John", LastName = "Doe", Email = "john@example.com" },
                    Course = new CourseDto { CourseName = "CS101", InstructorName = "Dr. A" }
                }
            },
            TotalItems = 1
        };

        _mockRegistrationService
            .Setup(s => s.GetRegistrationsAsync(1, int.MaxValue, null, null, null))
            .ReturnsAsync(registrations);

        // Act
        var result = await _controller.DownloadRegistrations();

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);
        Assert.StartsWith("registrations_", fileResult.FileDownloadName);
        Assert.EndsWith(".csv", fileResult.FileDownloadName);
    }

    [Fact]
    public async Task DownloadRegistrations_CsvContent_ContainsHeaderRow()
    {
        // Arrange
        var registrations = new PagedResponseDto<RegistrationDto>
        {
            Items = Enumerable.Empty<RegistrationDto>(),
            TotalItems = 0
        };

        _mockRegistrationService
            .Setup(s => s.GetRegistrationsAsync(1, int.MaxValue, null, null, null))
            .ReturnsAsync(registrations);

        // Act
        var result = await _controller.DownloadRegistrations();

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        var content = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);
        Assert.Contains("RegistrationId", content);
        Assert.Contains("StudentName", content);
        Assert.Contains("CourseName", content);
        Assert.Contains("Status", content);
    }

    [Fact]
    public async Task DownloadRegistrations_CsvContent_ContainsRegistrationData()
    {
        // Arrange
        var registrationId = Guid.NewGuid();
        var registrations = new PagedResponseDto<RegistrationDto>
        {
            Items = new List<RegistrationDto>
            {
                new RegistrationDto
                {
                    RegistrationId = registrationId,
                    StudentId = Guid.NewGuid(),
                    CourseId = Guid.NewGuid(),
                    RegistrationDate = new DateTime(2024, 3, 1),
                    Status = RegistrationStatus.Confirmed,
                    Student = new StudentDto { FirstName = "Alice", LastName = "Brown", Email = "alice@example.com" },
                    Course = new CourseDto { CourseName = "Advanced Math", InstructorName = "Prof. Davis" }
                }
            },
            TotalItems = 1
        };

        _mockRegistrationService
            .Setup(s => s.GetRegistrationsAsync(1, int.MaxValue, null, null, null))
            .ReturnsAsync(registrations);

        // Act
        var result = await _controller.DownloadRegistrations();

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        var content = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);
        Assert.Contains(registrationId.ToString(), content);
        Assert.Contains("Alice Brown", content);
        Assert.Contains("Advanced Math", content);
        Assert.Contains("Confirmed", content);
    }

    [Fact]
    public async Task DownloadCourses_CsvField_EscapesCommasInFields()
    {
        // Arrange
        var courses = new PagedResponseDto<CourseDto>
        {
            Items = new List<CourseDto>
            {
                new CourseDto
                {
                    CourseId = Guid.NewGuid(),
                    CourseName = "Math, Science, and Art",
                    InstructorName = "Dr. Smith",
                    Description = "A course covering math, science",
                    StartDate = new DateTime(2024, 1, 1),
                    EndDate = new DateTime(2024, 6, 1),
                    Schedule = "MWF",
                    CreatedAt = new DateTime(2024, 1, 1)
                }
            },
            TotalItems = 1
        };

        _mockCourseService
            .Setup(s => s.GetCoursesAsync(1, int.MaxValue, null, null))
            .ReturnsAsync(courses);

        // Act
        var result = await _controller.DownloadCourses();

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        var content = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);
        // Fields with commas should be quoted
        Assert.Contains("\"Math, Science, and Art\"", content);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenCourseServiceIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DownloadController(
            null!,
            _mockStudentService.Object,
            _mockRegistrationService.Object,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenStudentServiceIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DownloadController(
            _mockCourseService.Object,
            null!,
            _mockRegistrationService.Object,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenRegistrationServiceIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DownloadController(
            _mockCourseService.Object,
            _mockStudentService.Object,
            null!,
            _mockLogger.Object));
    }
}
