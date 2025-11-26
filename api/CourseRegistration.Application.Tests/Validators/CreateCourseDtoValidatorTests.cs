using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Validators;
using FluentAssertions;
using Xunit;

namespace CourseRegistration.Application.Tests.Validators;

/// <summary>
/// Unit tests for CreateCourseDtoValidator
/// </summary>
public class CreateCourseDtoValidatorTests
{
    private readonly CreateCourseDtoValidator _validator;

    public CreateCourseDtoValidatorTests()
    {
        _validator = new CreateCourseDtoValidator();
    }

    [Fact]
    public void Validate_WithValidDto_ShouldPass()
    {
        // Arrange
        var dto = new CreateCourseDto
        {
            CourseName = "Introduction to C#",
            Description = "Learn the fundamentals of C# programming",
            InstructorName = "John Doe",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(40),
            Schedule = "MWF 10:00-11:30"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithEmptyCourseName_ShouldFail()
    {
        // Arrange
        var dto = new CreateCourseDto
        {
            CourseName = "",
            Description = "Description",
            InstructorName = "John Doe",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(40),
            Schedule = "MWF 10:00-11:30"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CourseName" && e.ErrorMessage.Contains("required"));
    }

    [Fact]
    public void Validate_WithTooLongCourseName_ShouldFail()
    {
        // Arrange
        var dto = new CreateCourseDto
        {
            CourseName = new string('A', 101),
            Description = "Description",
            InstructorName = "John Doe",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(40),
            Schedule = "MWF 10:00-11:30"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CourseName" && e.ErrorMessage.Contains("100 characters"));
    }

    [Fact]
    public void Validate_WithStartDateInPast_ShouldFail()
    {
        // Arrange
        var dto = new CreateCourseDto
        {
            CourseName = "Valid Course",
            Description = "Description",
            InstructorName = "John Doe",
            StartDate = DateTime.UtcNow.AddDays(-5),
            EndDate = DateTime.UtcNow.AddDays(40),
            Schedule = "MWF 10:00-11:30"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StartDate" && e.ErrorMessage.Contains("future"));
    }

    [Fact]
    public void Validate_WithEndDateBeforeStartDate_ShouldFail()
    {
        // Arrange
        var dto = new CreateCourseDto
        {
            CourseName = "Valid Course",
            Description = "Description",
            InstructorName = "John Doe",
            StartDate = DateTime.UtcNow.AddDays(40),
            EndDate = DateTime.UtcNow.AddDays(10),
            Schedule = "MWF 10:00-11:30"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "EndDate" && e.ErrorMessage.Contains("after start date"));
    }
}
