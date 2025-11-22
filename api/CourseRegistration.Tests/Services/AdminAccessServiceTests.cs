using CourseRegistration.Application.Services;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CourseRegistration.Tests.Services;

/// <summary>
/// Unit tests for AdminAccessService with comprehensive edge case coverage
/// </summary>
public class AdminAccessServiceTests
{
    private readonly Mock<ILogger<AdminAccessService>> _loggerMock;
    private readonly AdminAccessService _service;

    public AdminAccessServiceTests()
    {
        _loggerMock = new Mock<ILogger<AdminAccessService>>();
        _service = new AdminAccessService(_loggerMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AdminAccessService(null!));
    }

    #endregion

    #region HasAdminAccess Tests

    [Fact]
    public void HasAdminAccess_WithNullUser_ReturnsFalse()
    {
        // Act
        var result = _service.HasAdminAccess(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasAdminAccess_WithAdminUser_ReturnsTrue()
    {
        // Arrange
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@test.com",
            Role = UserRole.Admin,
            IsActive = true
        };

        // Act
        var result = _service.HasAdminAccess(user);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasAdminAccess_WithRegularUser_ReturnsFalse()
    {
        // Arrange
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = "user",
            Email = "user@test.com",
            Role = UserRole.User,
            IsActive = true
        };

        // Act
        var result = _service.HasAdminAccess(user);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasAdminAccess_WithInstructorUser_ReturnsFalse()
    {
        // Arrange
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = "instructor",
            Email = "instructor@test.com",
            Role = UserRole.Instructor,
            IsActive = true
        };

        // Act
        var result = _service.HasAdminAccess(user);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasAdminAccess_WithInactiveAdminUser_ReturnsFalse()
    {
        // Arrange
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@test.com",
            Role = UserRole.Admin,
            IsActive = false
        };

        // Act
        var result = _service.HasAdminAccess(user);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasAdminAccess_WithEmptyUserId_ReturnsFalse()
    {
        // Arrange
        var user = new User
        {
            UserId = Guid.Empty,
            Username = "admin",
            Email = "admin@test.com",
            Role = UserRole.Admin,
            IsActive = true
        };

        // Act
        var result = _service.HasAdminAccess(user);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasAdminAccess_WithActiveAdminUser_LogsInformationMessage()
    {
        // Arrange
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@test.com",
            Role = UserRole.Admin,
            IsActive = true
        };

        // Act
        _service.HasAdminAccess(user);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Admin access granted")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void HasAdminAccess_WithNonAdminUser_LogsDenialMessage()
    {
        // Arrange
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = "user",
            Email = "user@test.com",
            Role = UserRole.User,
            IsActive = true
        };

        // Act
        _service.HasAdminAccess(user);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Admin access denied")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void HasAdminAccess_WithNullUser_LogsWarningMessage()
    {
        // Act
        _service.HasAdminAccess(null);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("User is null")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void HasAdminAccess_WithInactiveUser_LogsWarningMessage()
    {
        // Arrange
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@test.com",
            Role = UserRole.Admin,
            IsActive = false
        };

        // Act
        _service.HasAdminAccess(user);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("is not active")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region HasAdminAccessAsync Tests

    [Fact]
    public async Task HasAdminAccessAsync_WithEmptyGuid_ReturnsFalse()
    {
        // Act
        var result = await _service.HasAdminAccessAsync(Guid.Empty);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasAdminAccessAsync_WithValidGuid_ReturnsFalseForNow()
    {
        // This test validates current behavior - method returns false without DB context
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _service.HasAdminAccessAsync(userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasAdminAccessAsync_WithEmptyGuid_LogsWarningMessage()
    {
        // Act
        await _service.HasAdminAccessAsync(Guid.Empty);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Invalid user ID")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region Edge Case Tests

    [Theory]
    [InlineData(UserRole.User)]
    [InlineData(UserRole.Instructor)]
    public void HasAdminAccess_WithNonAdminRoles_ReturnsFalse(UserRole role)
    {
        // Arrange
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@test.com",
            Role = role,
            IsActive = true
        };

        // Act
        var result = _service.HasAdminAccess(user);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasAdminAccess_CalledMultipleTimes_ReturnsConsistentResults()
    {
        // Arrange
        var adminUser = new User
        {
            UserId = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@test.com",
            Role = UserRole.Admin,
            IsActive = true
        };

        // Act
        var result1 = _service.HasAdminAccess(adminUser);
        var result2 = _service.HasAdminAccess(adminUser);
        var result3 = _service.HasAdminAccess(adminUser);

        // Assert
        Assert.True(result1);
        Assert.True(result2);
        Assert.True(result3);
    }

    [Fact]
    public void HasAdminAccess_WithUserHavingSpecialCharactersInUsername_WorksCorrectly()
    {
        // Arrange
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = "admin@#$%",
            Email = "admin@test.com",
            Role = UserRole.Admin,
            IsActive = true
        };

        // Act
        var result = _service.HasAdminAccess(user);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasAdminAccess_WithMinimalValidAdminUser_ReturnsTrue()
    {
        // Test with only required fields set
        // Arrange
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Role = UserRole.Admin,
            IsActive = true
        };

        // Act
        var result = _service.HasAdminAccess(user);

        // Assert
        Assert.True(result);
    }

    #endregion
}
