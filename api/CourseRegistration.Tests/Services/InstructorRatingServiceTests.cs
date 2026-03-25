using Xunit;
using Moq;
using AutoMapper;
using CourseRegistration.Application.Services;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Mappings;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Interfaces;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Tests.Services;

/// <summary>
/// Unit tests for InstructorRatingService
/// </summary>
public class InstructorRatingServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly IMapper _mapper;
    private readonly InstructorRatingService _service;
    private readonly Mock<IInstructorRatingRepository> _mockRatingRepository;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<IRegistrationRepository> _mockRegistrationRepository;

    public InstructorRatingServiceTests()
    {
        // Setup AutoMapper
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = config.CreateMapper();

        // Setup mocks
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRatingRepository = new Mock<IInstructorRatingRepository>();
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockRegistrationRepository = new Mock<IRegistrationRepository>();

        _mockUnitOfWork.Setup(u => u.InstructorRatings).Returns(_mockRatingRepository.Object);
        _mockUnitOfWork.Setup(u => u.Courses).Returns(_mockCourseRepository.Object);
        _mockUnitOfWork.Setup(u => u.Students).Returns(_mockStudentRepository.Object);
        _mockUnitOfWork.Setup(u => u.Registrations).Returns(_mockRegistrationRepository.Object);

        _service = new InstructorRatingService(_mockUnitOfWork.Object, _mapper);
    }

    [Fact]
    public async Task CreateRatingAsync_ValidData_ReturnsRatingDto()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var course = new Course
        {
            CourseId = courseId,
            CourseName = "Test Course",
            InstructorName = "Dr. Smith",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30),
            Schedule = "MWF 10-11",
            IsActive = true
        };
        var student = new Student
        {
            StudentId = studentId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            DateOfBirth = DateTime.UtcNow.AddYears(-20),
            IsActive = true
        };

        var createDto = new CreateInstructorRatingDto
        {
            CourseId = courseId,
            StudentId = studentId,
            Rating = 5,
            Comment = "Excellent instructor!"
        };

        _mockCourseRepository.Setup(r => r.GetByIdAsync(courseId)).ReturnsAsync(course);
        _mockStudentRepository.Setup(r => r.GetByIdAsync(studentId)).ReturnsAsync(student);
        _mockRatingRepository.Setup(r => r.GetByStudentAndCourseAsync(studentId, courseId))
            .ReturnsAsync((InstructorRating?)null);
        _mockRegistrationRepository.Setup(r => r.IsStudentRegisteredForCourseAsync(studentId, courseId))
            .ReturnsAsync(true);
        _mockRatingRepository.Setup(r => r.AddAsync(It.IsAny<InstructorRating>()))
            .ReturnsAsync((InstructorRating rating) => rating);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _mockRatingRepository.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => new InstructorRating
            {
                RatingId = id,
                CourseId = courseId,
                StudentId = studentId,
                Rating = 5,
                Comment = "Excellent instructor!",
                Course = course,
                Student = student
            });

        // Act
        var result = await _service.CreateRatingAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Rating);
        Assert.Equal("Excellent instructor!", result.Comment);
        Assert.Equal("Test Course", result.CourseName);
        Assert.Equal("Dr. Smith", result.InstructorName);
        Assert.Equal("John Doe", result.StudentName);
        _mockRatingRepository.Verify(r => r.AddAsync(It.IsAny<InstructorRating>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateRatingAsync_CourseNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var createDto = new CreateInstructorRatingDto
        {
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Great!"
        };

        _mockCourseRepository.Setup(r => r.GetByIdAsync(createDto.CourseId))
            .ReturnsAsync((Course?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.CreateRatingAsync(createDto));
    }

    [Fact]
    public async Task CreateRatingAsync_StudentNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var createDto = new CreateInstructorRatingDto
        {
            CourseId = courseId,
            StudentId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Great!"
        };

        var course = new Course
        {
            CourseId = courseId,
            CourseName = "Test Course",
            InstructorName = "Dr. Smith",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30),
            Schedule = "MWF 10-11",
            IsActive = true
        };

        _mockCourseRepository.Setup(r => r.GetByIdAsync(courseId)).ReturnsAsync(course);
        _mockStudentRepository.Setup(r => r.GetByIdAsync(createDto.StudentId))
            .ReturnsAsync((Student?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.CreateRatingAsync(createDto));
    }

    [Fact]
    public async Task CreateRatingAsync_StudentAlreadyRated_ThrowsInvalidOperationException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var course = new Course
        {
            CourseId = courseId,
            CourseName = "Test Course",
            InstructorName = "Dr. Smith",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30),
            Schedule = "MWF 10-11",
            IsActive = true
        };
        var student = new Student
        {
            StudentId = studentId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            DateOfBirth = DateTime.UtcNow.AddYears(-20),
            IsActive = true
        };

        var createDto = new CreateInstructorRatingDto
        {
            CourseId = courseId,
            StudentId = studentId,
            Rating = 5,
            Comment = "Great!"
        };

        var existingRating = new InstructorRating
        {
            RatingId = Guid.NewGuid(),
            CourseId = courseId,
            StudentId = studentId,
            Rating = 4,
            Comment = "Good"
        };

        _mockCourseRepository.Setup(r => r.GetByIdAsync(courseId)).ReturnsAsync(course);
        _mockStudentRepository.Setup(r => r.GetByIdAsync(studentId)).ReturnsAsync(student);
        _mockRatingRepository.Setup(r => r.GetByStudentAndCourseAsync(studentId, courseId))
            .ReturnsAsync(existingRating);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.CreateRatingAsync(createDto));
    }

    [Fact]
    public async Task CreateRatingAsync_StudentNotEnrolled_ThrowsInvalidOperationException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var course = new Course
        {
            CourseId = courseId,
            CourseName = "Test Course",
            InstructorName = "Dr. Smith",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30),
            Schedule = "MWF 10-11",
            IsActive = true
        };
        var student = new Student
        {
            StudentId = studentId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            DateOfBirth = DateTime.UtcNow.AddYears(-20),
            IsActive = true
        };

        var createDto = new CreateInstructorRatingDto
        {
            CourseId = courseId,
            StudentId = studentId,
            Rating = 5,
            Comment = "Great!"
        };

        _mockCourseRepository.Setup(r => r.GetByIdAsync(courseId)).ReturnsAsync(course);
        _mockStudentRepository.Setup(r => r.GetByIdAsync(studentId)).ReturnsAsync(student);
        _mockRatingRepository.Setup(r => r.GetByStudentAndCourseAsync(studentId, courseId))
            .ReturnsAsync((InstructorRating?)null);
        _mockRegistrationRepository.Setup(r => r.IsStudentRegisteredForCourseAsync(studentId, courseId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.CreateRatingAsync(createDto));
    }

    [Fact]
    public async Task GetRatingByIdAsync_ValidId_ReturnsRatingDto()
    {
        // Arrange
        var ratingId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        
        var rating = new InstructorRating
        {
            RatingId = ratingId,
            CourseId = courseId,
            StudentId = studentId,
            Rating = 4,
            Comment = "Good course",
            Course = new Course
            {
                CourseId = courseId,
                CourseName = "Test Course",
                InstructorName = "Dr. Smith",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30),
                Schedule = "MWF 10-11"
            },
            Student = new Student
            {
                StudentId = studentId,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@test.com",
                DateOfBirth = DateTime.UtcNow.AddYears(-22)
            }
        };

        _mockRatingRepository.Setup(r => r.GetWithDetailsAsync(ratingId)).ReturnsAsync(rating);

        // Act
        var result = await _service.GetRatingByIdAsync(ratingId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ratingId, result.RatingId);
        Assert.Equal(4, result.Rating);
        Assert.Equal("Good course", result.Comment);
        Assert.Equal("Test Course", result.CourseName);
        Assert.Equal("Dr. Smith", result.InstructorName);
        Assert.Equal("Jane Smith", result.StudentName);
    }

    [Fact]
    public async Task GetRatingByIdAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        var ratingId = Guid.NewGuid();
        _mockRatingRepository.Setup(r => r.GetWithDetailsAsync(ratingId))
            .ReturnsAsync((InstructorRating?)null);

        // Act
        var result = await _service.GetRatingByIdAsync(ratingId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateRatingAsync_ValidData_ReturnsUpdatedRatingDto()
    {
        // Arrange
        var ratingId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var studentId = Guid.NewGuid();

        var existingRating = new InstructorRating
        {
            RatingId = ratingId,
            CourseId = courseId,
            StudentId = studentId,
            Rating = 4,
            Comment = "Good"
        };

        var updateDto = new UpdateInstructorRatingDto
        {
            Rating = 5,
            Comment = "Excellent!"
        };

        var updatedRating = new InstructorRating
        {
            RatingId = ratingId,
            CourseId = courseId,
            StudentId = studentId,
            Rating = 5,
            Comment = "Excellent!",
            Course = new Course
            {
                CourseId = courseId,
                CourseName = "Test Course",
                InstructorName = "Dr. Smith",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30),
                Schedule = "MWF 10-11"
            },
            Student = new Student
            {
                StudentId = studentId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@test.com",
                DateOfBirth = DateTime.UtcNow.AddYears(-20)
            }
        };

        _mockRatingRepository.Setup(r => r.GetByIdAsync(ratingId)).ReturnsAsync(existingRating);
        _mockRatingRepository.Setup(r => r.Update(It.IsAny<InstructorRating>()));
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _mockRatingRepository.Setup(r => r.GetWithDetailsAsync(ratingId)).ReturnsAsync(updatedRating);

        // Act
        var result = await _service.UpdateRatingAsync(ratingId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Rating);
        Assert.Equal("Excellent!", result.Comment);
        _mockRatingRepository.Verify(r => r.Update(It.IsAny<InstructorRating>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateRatingAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        var ratingId = Guid.NewGuid();
        var updateDto = new UpdateInstructorRatingDto
        {
            Rating = 5,
            Comment = "Excellent!"
        };

        _mockRatingRepository.Setup(r => r.GetByIdAsync(ratingId))
            .ReturnsAsync((InstructorRating?)null);

        // Act
        var result = await _service.UpdateRatingAsync(ratingId, updateDto);

        // Assert
        Assert.Null(result);
        _mockRatingRepository.Verify(r => r.Update(It.IsAny<InstructorRating>()), Times.Never);
    }

    [Fact]
    public async Task DeleteRatingAsync_ValidId_ReturnsTrue()
    {
        // Arrange
        var ratingId = Guid.NewGuid();
        var rating = new InstructorRating
        {
            RatingId = ratingId,
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Rating = 4,
            Comment = "Good"
        };

        _mockRatingRepository.Setup(r => r.GetByIdAsync(ratingId)).ReturnsAsync(rating);
        _mockRatingRepository.Setup(r => r.Remove(rating));
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.DeleteRatingAsync(ratingId);

        // Assert
        Assert.True(result);
        _mockRatingRepository.Verify(r => r.Remove(rating), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteRatingAsync_InvalidId_ReturnsFalse()
    {
        // Arrange
        var ratingId = Guid.NewGuid();
        _mockRatingRepository.Setup(r => r.GetByIdAsync(ratingId))
            .ReturnsAsync((InstructorRating?)null);

        // Act
        var result = await _service.DeleteRatingAsync(ratingId);

        // Assert
        Assert.False(result);
        _mockRatingRepository.Verify(r => r.Remove(It.IsAny<InstructorRating>()), Times.Never);
    }

    [Fact]
    public async Task GetInstructorStatsAsync_ValidInstructor_ReturnsStats()
    {
        // Arrange
        var instructorName = "Dr. Smith";
        var courseId1 = Guid.NewGuid();
        var courseId2 = Guid.NewGuid();

        var ratings = new List<InstructorRating>
        {
            new InstructorRating
            {
                RatingId = Guid.NewGuid(),
                CourseId = courseId1,
                StudentId = Guid.NewGuid(),
                Rating = 5,
                Course = new Course { CourseId = courseId1, InstructorName = instructorName, CourseName = "Course1", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), Schedule = "MWF" }
            },
            new InstructorRating
            {
                RatingId = Guid.NewGuid(),
                CourseId = courseId2,
                StudentId = Guid.NewGuid(),
                Rating = 4,
                Course = new Course { CourseId = courseId2, InstructorName = instructorName, CourseName = "Course2", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), Schedule = "TTh" }
            },
            new InstructorRating
            {
                RatingId = Guid.NewGuid(),
                CourseId = courseId1,
                StudentId = Guid.NewGuid(),
                Rating = 5,
                Course = new Course { CourseId = courseId1, InstructorName = instructorName, CourseName = "Course1", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), Schedule = "MWF" }
            }
        };

        var courses = new List<Course>
        {
            new Course { CourseId = courseId1, InstructorName = instructorName, CourseName = "Course1", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), Schedule = "MWF" },
            new Course { CourseId = courseId2, InstructorName = instructorName, CourseName = "Course2", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), Schedule = "TTh" }
        };

        _mockRatingRepository.Setup(r => r.GetByInstructorNameAsync(instructorName))
            .ReturnsAsync(ratings);
        _mockCourseRepository.Setup(r => r.GetCoursesByInstructorAsync(instructorName))
            .ReturnsAsync(courses);

        // Act
        var result = await _service.GetInstructorStatsAsync(instructorName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(instructorName, result.InstructorName);
        Assert.Equal(4.67, Math.Round(result.AverageRating, 2)); // (5 + 4 + 5) / 3 = 4.67
        Assert.Equal(3, result.TotalRatings);
        Assert.Equal(2, result.FiveStarCount);
        Assert.Equal(1, result.FourStarCount);
        Assert.Equal(0, result.ThreeStarCount);
        Assert.Equal(2, result.CourseIds.Count);
    }

    [Fact]
    public async Task GetInstructorStatsAsync_NoRatings_ReturnsNull()
    {
        // Arrange
        var instructorName = "Dr. Nobody";
        _mockRatingRepository.Setup(r => r.GetByInstructorNameAsync(instructorName))
            .ReturnsAsync(new List<InstructorRating>());

        // Act
        var result = await _service.GetInstructorStatsAsync(instructorName);

        // Assert
        Assert.Null(result);
    }
}
