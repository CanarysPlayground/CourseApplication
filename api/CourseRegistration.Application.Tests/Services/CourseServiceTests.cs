using AutoMapper;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Services;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CourseRegistration.Application.Tests.Services;

/// <summary>
/// Unit tests for CourseService focusing on CreateCourseAsync functionality
/// </summary>
public class CourseServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly CourseService _courseService;

    public CourseServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockCourseRepository = new Mock<ICourseRepository>();
        
        _mockUnitOfWork.Setup(u => u.Courses).Returns(_mockCourseRepository.Object);
        
        _courseService = new CourseService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateCourseAsync_WithValidData_ShouldCreateCourse()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            CourseName = "Introduction to C#",
            Description = "Learn the fundamentals of C# programming",
            InstructorName = "John Doe",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(40),
            Schedule = "MWF 10:00-11:30"
        };

        var course = new Course
        {
            CourseId = Guid.NewGuid(),
            CourseName = createCourseDto.CourseName,
            Description = createCourseDto.Description,
            InstructorName = createCourseDto.InstructorName,
            StartDate = createCourseDto.StartDate,
            EndDate = createCourseDto.EndDate,
            Schedule = createCourseDto.Schedule
        };

        var courseDto = new CourseDto
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            Description = course.Description,
            InstructorName = course.InstructorName,
            StartDate = course.StartDate,
            EndDate = course.EndDate,
            Schedule = course.Schedule
        };

        _mockMapper.Setup(m => m.Map<Course>(It.IsAny<CreateCourseDto>())).Returns(course);
        _mockMapper.Setup(m => m.Map<CourseDto>(It.IsAny<Course>())).Returns(courseDto);
        _mockCourseRepository.Setup(r => r.AddAsync(It.IsAny<Course>())).ReturnsAsync(course);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _courseService.CreateCourseAsync(createCourseDto);

        // Assert
        result.Should().NotBeNull();
        result.CourseName.Should().Be(createCourseDto.CourseName);
        result.Description.Should().Be(createCourseDto.Description);
        result.InstructorName.Should().Be(createCourseDto.InstructorName);
        result.StartDate.Should().Be(createCourseDto.StartDate);
        result.EndDate.Should().Be(createCourseDto.EndDate);
        result.Schedule.Should().Be(createCourseDto.Schedule);

        _mockCourseRepository.Verify(r => r.AddAsync(It.IsAny<Course>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateCourseAsync_WithStartDateInPast_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            CourseName = "Introduction to C#",
            Description = "Learn the fundamentals of C# programming",
            InstructorName = "John Doe",
            StartDate = DateTime.UtcNow.AddDays(-5), // Past date
            EndDate = DateTime.UtcNow.AddDays(30),
            Schedule = "MWF 10:00-11:30"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _courseService.CreateCourseAsync(createCourseDto));
        
        exception.Message.Should().Contain("must be in the future");

        _mockCourseRepository.Verify(r => r.AddAsync(It.IsAny<Course>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateCourseAsync_WithEndDateBeforeStartDate_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            CourseName = "Introduction to C#",
            Description = "Learn the fundamentals of C# programming",
            InstructorName = "John Doe",
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(10), // Before start date
            Schedule = "MWF 10:00-11:30"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _courseService.CreateCourseAsync(createCourseDto));
        
        exception.Message.Should().Contain("must be after start date");

        _mockCourseRepository.Verify(r => r.AddAsync(It.IsAny<Course>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateCourseAsync_WithEndDateEqualToStartDate_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(10);
        var createCourseDto = new CreateCourseDto
        {
            CourseName = "Introduction to C#",
            Description = "Learn the fundamentals of C# programming",
            InstructorName = "John Doe",
            StartDate = startDate,
            EndDate = startDate, // Same as start date
            Schedule = "MWF 10:00-11:30"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _courseService.CreateCourseAsync(createCourseDto));
        
        exception.Message.Should().Contain("must be after start date");

        _mockCourseRepository.Verify(r => r.AddAsync(It.IsAny<Course>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateCourseAsync_WithMinimalData_ShouldCreateCourse()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            CourseName = "Short Course",
            Description = null, // Optional field
            InstructorName = "Jane Smith",
            StartDate = DateTime.UtcNow.AddDays(7),
            EndDate = DateTime.UtcNow.AddDays(14),
            Schedule = "TTh 14:00-15:30"
        };

        var course = new Course
        {
            CourseId = Guid.NewGuid(),
            CourseName = createCourseDto.CourseName,
            Description = createCourseDto.Description,
            InstructorName = createCourseDto.InstructorName,
            StartDate = createCourseDto.StartDate,
            EndDate = createCourseDto.EndDate,
            Schedule = createCourseDto.Schedule
        };

        var courseDto = new CourseDto
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName,
            Description = course.Description,
            InstructorName = course.InstructorName,
            StartDate = course.StartDate,
            EndDate = course.EndDate,
            Schedule = course.Schedule
        };

        _mockMapper.Setup(m => m.Map<Course>(It.IsAny<CreateCourseDto>())).Returns(course);
        _mockMapper.Setup(m => m.Map<CourseDto>(It.IsAny<Course>())).Returns(courseDto);
        _mockCourseRepository.Setup(r => r.AddAsync(It.IsAny<Course>())).ReturnsAsync(course);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _courseService.CreateCourseAsync(createCourseDto);

        // Assert
        result.Should().NotBeNull();
        result.CourseName.Should().Be(createCourseDto.CourseName);
        result.Description.Should().BeNull();
        
        _mockCourseRepository.Verify(r => r.AddAsync(It.IsAny<Course>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateCourseAsync_ShouldGenerateNewCourseId()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            CourseName = "Advanced C#",
            Description = "Deep dive into C# features",
            InstructorName = "Dr. Smith",
            StartDate = DateTime.UtcNow.AddDays(15),
            EndDate = DateTime.UtcNow.AddDays(60),
            Schedule = "MWF 13:00-14:30"
        };

        var capturedCourse = (Course?)null;
        
        _mockMapper.Setup(m => m.Map<Course>(It.IsAny<CreateCourseDto>()))
            .Returns((CreateCourseDto dto) => new Course
            {
                CourseId = Guid.NewGuid(),
                CourseName = dto.CourseName,
                Description = dto.Description,
                InstructorName = dto.InstructorName,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Schedule = dto.Schedule
            });

        _mockCourseRepository.Setup(r => r.AddAsync(It.IsAny<Course>()))
            .Callback<Course>(c => capturedCourse = c)
            .ReturnsAsync((Course c) => c);

        _mockMapper.Setup(m => m.Map<CourseDto>(It.IsAny<Course>()))
            .Returns((Course c) => new CourseDto
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                Description = c.Description,
                InstructorName = c.InstructorName,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Schedule = c.Schedule
            });

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _courseService.CreateCourseAsync(createCourseDto);

        // Assert
        result.Should().NotBeNull();
        result.CourseId.Should().NotBe(Guid.Empty);
        capturedCourse.Should().NotBeNull();
        capturedCourse!.CourseId.Should().NotBe(Guid.Empty);
    }
}
