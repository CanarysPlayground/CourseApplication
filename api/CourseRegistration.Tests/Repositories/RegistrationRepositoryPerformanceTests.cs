using Xunit;
using Microsoft.EntityFrameworkCore;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Enums;
using CourseRegistration.Infrastructure.Data;
using CourseRegistration.Infrastructure.Repositories;

namespace CourseRegistration.Tests.Repositories;

/// <summary>
/// Performance-focused tests for RegistrationRepository
/// Validates that pagination and filtering are efficient
/// </summary>
public class RegistrationRepositoryPerformanceTests
{
    private CourseRegistrationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<CourseRegistrationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new CourseRegistrationDbContext(options);
    }

    [Fact]
    public async Task GetPagedRegistrationsWithFiltersAsync_WithFilters_ReturnsPaginatedResults()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new RegistrationRepository(context);

        var student1 = new Student
        {
            StudentId = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            IsActive = true
        };

        var student2 = new Student
        {
            StudentId = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com",
            IsActive = true
        };

        var course = new Course
        {
            CourseId = Guid.NewGuid(),
            CourseName = "Test Course",
            Description = "Test Description",
            InstructorName = "Test Instructor",
            Schedule = "MWF 10:00-11:00",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(40),
            IsActive = true
        };

        await context.Students.AddAsync(student1);
        await context.Students.AddAsync(student2);
        await context.Courses.AddAsync(course);

        // Create 15 registrations for student1
        for (int i = 0; i < 15; i++)
        {
            await context.Registrations.AddAsync(new Registration
            {
                RegistrationId = Guid.NewGuid(),
                StudentId = student1.StudentId,
                CourseId = course.CourseId,
                Status = RegistrationStatus.Pending,
                RegistrationDate = DateTime.UtcNow.AddDays(-i)
            });
        }

        // Create 5 registrations for student2
        for (int i = 0; i < 5; i++)
        {
            await context.Registrations.AddAsync(new Registration
            {
                RegistrationId = Guid.NewGuid(),
                StudentId = student2.StudentId,
                CourseId = course.CourseId,
                Status = RegistrationStatus.Confirmed,
                RegistrationDate = DateTime.UtcNow.AddDays(-i)
            });
        }

        await context.SaveChangesAsync();

        // Act - Get first page with 10 items filtered by student1
        var result = await repository.GetPagedRegistrationsWithFiltersAsync(
            page: 1,
            pageSize: 10,
            studentId: student1.StudentId);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(10, resultList.Count); // Should return 10 items (pageSize)
        Assert.All(resultList, r => Assert.Equal(student1.StudentId, r.StudentId));
    }

    [Fact]
    public async Task GetPagedRegistrationsWithFiltersAsync_SecondPage_ReturnsRemainingResults()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new RegistrationRepository(context);

        var student = new Student
        {
            StudentId = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            IsActive = true
        };

        var course = new Course
        {
            CourseId = Guid.NewGuid(),
            CourseName = "Test Course",
            Description = "Test Description",
            InstructorName = "Test Instructor",
            Schedule = "MWF 10:00-11:00",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(40),
            IsActive = true
        };

        await context.Students.AddAsync(student);
        await context.Courses.AddAsync(course);

        // Create 15 registrations
        for (int i = 0; i < 15; i++)
        {
            await context.Registrations.AddAsync(new Registration
            {
                RegistrationId = Guid.NewGuid(),
                StudentId = student.StudentId,
                CourseId = course.CourseId,
                Status = RegistrationStatus.Pending,
                RegistrationDate = DateTime.UtcNow.AddDays(-i)
            });
        }

        await context.SaveChangesAsync();

        // Act - Get second page with 10 items per page
        var result = await repository.GetPagedRegistrationsWithFiltersAsync(
            page: 2,
            pageSize: 10,
            studentId: student.StudentId);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(5, resultList.Count); // Should return 5 remaining items
    }

    [Fact]
    public async Task CountRegistrationsWithFiltersAsync_WithFilters_ReturnsCorrectCount()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new RegistrationRepository(context);

        var student1 = new Student
        {
            StudentId = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            IsActive = true
        };

        var student2 = new Student
        {
            StudentId = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com",
            IsActive = true
        };

        var course = new Course
        {
            CourseId = Guid.NewGuid(),
            CourseName = "Test Course",
            Description = "Test Description",
            InstructorName = "Test Instructor",
            Schedule = "MWF 10:00-11:00",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(40),
            IsActive = true
        };

        await context.Students.AddAsync(student1);
        await context.Students.AddAsync(student2);
        await context.Courses.AddAsync(course);

        // Create 15 registrations for student1
        for (int i = 0; i < 15; i++)
        {
            await context.Registrations.AddAsync(new Registration
            {
                RegistrationId = Guid.NewGuid(),
                StudentId = student1.StudentId,
                CourseId = course.CourseId,
                Status = RegistrationStatus.Pending,
                RegistrationDate = DateTime.UtcNow.AddDays(-i)
            });
        }

        // Create 5 registrations for student2
        for (int i = 0; i < 5; i++)
        {
            await context.Registrations.AddAsync(new Registration
            {
                RegistrationId = Guid.NewGuid(),
                StudentId = student2.StudentId,
                CourseId = course.CourseId,
                Status = RegistrationStatus.Confirmed,
                RegistrationDate = DateTime.UtcNow.AddDays(-i)
            });
        }

        await context.SaveChangesAsync();

        // Act
        var count = await repository.CountRegistrationsWithFiltersAsync(studentId: student1.StudentId);

        // Assert
        Assert.Equal(15, count);
    }

    [Fact]
    public async Task CountRegistrationsWithFiltersAsync_WithStatusFilter_ReturnsCorrectCount()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new RegistrationRepository(context);

        var student = new Student
        {
            StudentId = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            IsActive = true
        };

        var course = new Course
        {
            CourseId = Guid.NewGuid(),
            CourseName = "Test Course",
            Description = "Test Description",
            InstructorName = "Test Instructor",
            Schedule = "MWF 10:00-11:00",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(40),
            IsActive = true
        };

        await context.Students.AddAsync(student);
        await context.Courses.AddAsync(course);

        // Create 10 pending and 5 confirmed registrations
        for (int i = 0; i < 10; i++)
        {
            await context.Registrations.AddAsync(new Registration
            {
                RegistrationId = Guid.NewGuid(),
                StudentId = student.StudentId,
                CourseId = course.CourseId,
                Status = RegistrationStatus.Pending,
                RegistrationDate = DateTime.UtcNow.AddDays(-i)
            });
        }

        for (int i = 0; i < 5; i++)
        {
            await context.Registrations.AddAsync(new Registration
            {
                RegistrationId = Guid.NewGuid(),
                StudentId = student.StudentId,
                CourseId = course.CourseId,
                Status = RegistrationStatus.Confirmed,
                RegistrationDate = DateTime.UtcNow.AddDays(-i)
            });
        }

        await context.SaveChangesAsync();

        // Act
        var pendingCount = await repository.CountRegistrationsWithFiltersAsync(status: RegistrationStatus.Pending);
        var confirmedCount = await repository.CountRegistrationsWithFiltersAsync(status: RegistrationStatus.Confirmed);

        // Assert
        Assert.Equal(10, pendingCount);
        Assert.Equal(5, confirmedCount);
    }
}
