using Xunit;
using Microsoft.EntityFrameworkCore;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Infrastructure.Data;
using CourseRegistration.Infrastructure.Repositories;
using CourseRegistration.Infrastructure.Utilities;

namespace CourseRegistration.Tests.Repositories;

/// <summary>
/// Security tests for repository search operations
/// Validates that SQL LIKE wildcard characters are properly escaped
/// </summary>
public class RepositorySecurityTests
{
    private CourseRegistrationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<CourseRegistrationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new CourseRegistrationDbContext(options);
    }

    [Fact]
    public async Task SearchByNameAsync_WithPercentInSearchTerm_TreatsAsLiteral()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new StudentRepository(context);

        await context.Students.AddAsync(new Student
        {
            StudentId = Guid.NewGuid(),
            FirstName = "John%Test",
            LastName = "Doe",
            Email = "john@test.com",
            IsActive = true
        });

        await context.Students.AddAsync(new Student
        {
            StudentId = Guid.NewGuid(),
            FirstName = "JohnMatch",
            LastName = "Doe",
            Email = "johnmatch@test.com",
            IsActive = true
        });

        await context.SaveChangesAsync();

        // Act - Search for "%" which should be treated as literal, not wildcard
        var results = await repository.SearchByNameAsync("%");

        // Assert - Should only match the student with "%" in the name
        var resultList = results.ToList();
        Assert.Single(resultList);
        Assert.Contains("John%Test", resultList[0].FirstName);
    }

    [Fact]
    public async Task SearchByNameAsync_WithUnderscoreInSearchTerm_TreatsAsLiteral()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new StudentRepository(context);

        await context.Students.AddAsync(new Student
        {
            StudentId = Guid.NewGuid(),
            FirstName = "John_Test",
            LastName = "Doe",
            Email = "john@test.com",
            IsActive = true
        });

        await context.Students.AddAsync(new Student
        {
            StudentId = Guid.NewGuid(),
            FirstName = "JohnXTest",
            LastName = "Doe",
            Email = "johnx@test.com",
            IsActive = true
        });

        await context.SaveChangesAsync();

        // Act - Search for "_" which should be treated as literal, not single-character wildcard
        var results = await repository.SearchByNameAsync("_");

        // Assert - Should only match the student with "_" in the name
        var resultList = results.ToList();
        Assert.Single(resultList);
        Assert.Contains("John_Test", resultList[0].FirstName);
    }

    [Fact]
    public async Task SearchCoursesAsync_WithPercentInSearchTerm_TreatsAsLiteral()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new CourseRepository(context);

        await context.Courses.AddAsync(new Course
        {
            CourseId = Guid.NewGuid(),
            CourseName = "100% Success Course",
            Description = "A course about success",
            InstructorName = "Test Instructor",
            Schedule = "MWF 10:00-11:00",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(40),
            IsActive = true
        });

        await context.Courses.AddAsync(new Course
        {
            CourseId = Guid.NewGuid(),
            CourseName = "Success Course",
            Description = "Another course",
            InstructorName = "Test Instructor",
            Schedule = "TTh 14:00-15:30",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(40),
            IsActive = true
        });

        await context.SaveChangesAsync();

        // Act - Search for "100%" which should be treated as literal
        var results = await repository.SearchCoursesAsync("100%", null);

        // Assert - Should only match the course with "100%" in the name
        var resultList = results.ToList();
        Assert.Single(resultList);
        Assert.Contains("100%", resultList[0].CourseName);
    }

    [Fact]
    public async Task GetCoursesByInstructorAsync_WithSquareBracketInSearchTerm_TreatsAsLiteral()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new CourseRepository(context);

        await context.Courses.AddAsync(new Course
        {
            CourseId = Guid.NewGuid(),
            CourseName = "Test Course 1",
            Description = "Test Description",
            InstructorName = "Dr. [PhD] Smith",
            Schedule = "MWF 10:00-11:00",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(40),
            IsActive = true
        });

        await context.Courses.AddAsync(new Course
        {
            CourseId = Guid.NewGuid(),
            CourseName = "Test Course 2",
            Description = "Test Description",
            InstructorName = "Dr. PhD Smith",
            Schedule = "TTh 14:00-15:30",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(40),
            IsActive = true
        });

        await context.SaveChangesAsync();

        // Act - Search for "[PhD]" which should be treated as literal
        var results = await repository.GetCoursesByInstructorAsync("[PhD]");

        // Assert - Should only match the instructor with "[PhD]" in the name
        var resultList = results.ToList();
        Assert.Single(resultList);
        Assert.Contains("[PhD]", resultList[0].InstructorName);
    }

    [Fact]
    public async Task SearchByNameAsync_WithNormalText_WorksAsExpected()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new StudentRepository(context);

        await context.Students.AddAsync(new Student
        {
            StudentId = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            IsActive = true
        });

        await context.Students.AddAsync(new Student
        {
            StudentId = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@test.com",
            IsActive = true
        });

        await context.SaveChangesAsync();

        // Act
        var results = await repository.SearchByNameAsync("John");

        // Assert
        var resultList = results.ToList();
        Assert.Single(resultList);
        Assert.Equal("John", resultList[0].FirstName);
    }

    [Fact]
    public void QueryHelpers_EscapeLikePattern_WithNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => QueryHelpers.EscapeLikePattern(null!));
    }

    [Fact]
    public void QueryHelpers_EscapeLikePattern_EscapesAllSpecialCharacters()
    {
        // Arrange
        var input = "test^%_[]string";

        // Act
        var result = QueryHelpers.EscapeLikePattern(input);

        // Assert
        Assert.Equal("test^^^%^_^[^]string", result);
    }
}
