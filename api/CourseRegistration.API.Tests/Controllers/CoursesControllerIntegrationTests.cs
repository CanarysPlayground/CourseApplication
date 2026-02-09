using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CourseRegistration.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CourseRegistration.API.Tests.Controllers;

/// <summary>
/// Integration tests for CoursesController focusing on CreateCourse endpoint
/// </summary>
public class CoursesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public CoursesControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCourse_WithValidData_ShouldReturn201Created()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            CourseName = "Integration Test Course",
            Description = "This is a test course created during integration testing",
            InstructorName = "Dr. Test",
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(60),
            Schedule = "MWF 10:00-11:30"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/courses", createCourseDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
        
        var apiResponse = JsonSerializer.Deserialize<JsonElement>(content);
        apiResponse.GetProperty("success").GetBoolean().Should().BeTrue();
        apiResponse.GetProperty("data").GetProperty("courseName").GetString().Should().Be(createCourseDto.CourseName);
    }

    [Fact]
    public async Task CreateCourse_WithStartDateInPast_ShouldReturn400BadRequest()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            CourseName = "Invalid Course",
            Description = "Course with past start date",
            InstructorName = "Dr. Test",
            StartDate = DateTime.UtcNow.AddDays(-5),
            EndDate = DateTime.UtcNow.AddDays(30),
            Schedule = "MWF 10:00-11:30"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/courses", createCourseDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCourse_WithEndDateBeforeStartDate_ShouldReturn400BadRequest()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            CourseName = "Invalid Course",
            Description = "Course with end date before start date",
            InstructorName = "Dr. Test",
            StartDate = DateTime.UtcNow.AddDays(60),
            EndDate = DateTime.UtcNow.AddDays(30),
            Schedule = "MWF 10:00-11:30"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/courses", createCourseDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCourse_ShouldReturnLocationHeader()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            CourseName = "Location Header Test Course",
            Description = "Testing location header",
            InstructorName = "Dr. Location",
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(60),
            Schedule = "MWF 09:00-10:30"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/courses", createCourseDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain("/api/Courses/");
    }
}
