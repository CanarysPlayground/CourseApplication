using Xunit;
using CourseRegistration.Application.Services;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Tests.Services;

/// <summary>


/// Unit tests for admin access checking functionality
/// </summary>
public class AdminAccessCheckerTests
{
    [Fact]
    public void HasAdminAccess_WithAdminRole_ReturnsTrue()
    {
        // Arrange
        var adminUser = new User
        {
            UserId = Guid.NewGuid(),
            Role = UserRole.Admin,
            IsActive = true,
            Username = "admin",
            Email = "admin@test.com",
            FirstName = "Admin",
            LastName = "User"
        };

        // Act
        var result = AdminAccessChecker.HasAdminAccess(adminUser);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasAdminAccess_WithSuperAdminRole_ReturnsTrue()
    {
        // Arrange
        var superAdminUser = new User
        {
            UserId = Guid.NewGuid(),
            Role = UserRole.SuperAdmin,
            IsActive = true,
            Username = "superadmin",
            Email = "superadmin@test.com",
            FirstName = "Super",
            LastName = "Admin"
        };

        // Act
        var result = AdminAccessChecker.HasAdminAccess(superAdminUser);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasAdminAccess_WithStudentRole_ReturnsFalse()
    {
        // Arrange
        var studentUser = new User
        {
            UserId = Guid.NewGuid(),
            Role = UserRole.Student,
            IsActive = true,
            Username = "student",
            Email = "student@test.com",
            FirstName = "Student",
            LastName = "User"
        };

        // Act
        var result = AdminAccessChecker.HasAdminAccess(studentUser);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasAdminAccess_WithInstructorRole_ReturnsFalse()
    {
        // Arrange
        var instructorUser = new User
        {
            UserId = Guid.NewGuid(),
            Role = UserRole.Instructor,
            IsActive = true,
            Username = "instructor",
            Email = "instructor@test.com",
            FirstName = "Instructor",
            LastName = "User"
        };

        // Act
        var result = AdminAccessChecker.HasAdminAccess(instructorUser);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasAdminAccess_WithInactiveAdminUser_ReturnsFalse()
    {
        // Arrange
        var inactiveAdminUser = new User
        {
            UserId = Guid.NewGuid(),
            Role = UserRole.Admin,
            IsActive = false,
            Username = "inactiveadmin",
            Email = "inactiveadmin@test.com",
            FirstName = "Inactive",
            LastName = "Admin"
        };

        // Act
        var result = AdminAccessChecker.HasAdminAccess(inactiveAdminUser);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasAdminAccess_WithNullUser_ReturnsFalse()
    {
        // Act
        var result = AdminAccessChecker.HasAdminAccess(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasInstructorAccess_WithInstructorRole_ReturnsTrue()
    {
        // Arrange
        var instructorUser = new User
        {
            UserId = Guid.NewGuid(),
            Role = UserRole.Instructor,
            IsActive = true,
            Username = "instructor",
            Email = "instructor@test.com",
            FirstName = "Instructor",
            LastName = "User"
        };

        // Act
        var result = AdminAccessChecker.HasInstructorAccess(instructorUser);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasInstructorAccess_WithAdminRole_ReturnsTrue()
    {
        // Arrange
        var adminUser = new User
        {
            UserId = Guid.NewGuid(),
            Role = UserRole.Admin,
            IsActive = true,
            Username = "admin",
            Email = "admin@test.com",
            FirstName = "Admin",
            LastName = "User"
        };

        // Act
        var result = AdminAccessChecker.HasInstructorAccess(adminUser);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(UserRole.Admin, true)]
    [InlineData(UserRole.SuperAdmin, true)]
    [InlineData(UserRole.Instructor, false)]
    [InlineData(UserRole.Student, false)]
    public void HasAdminAccess_ByRole_ReturnsCorrectResult(UserRole role, bool expectedResult)
    {
        // Act
        var result = AdminAccessChecker.HasAdminAccess(role, isActive: true);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void CanChangeUserRole_SuperAdminChangingAnyRole_ReturnsTrue()
    {
        // Act & Assert
        Assert.True(AdminAccessChecker.CanChangeUserRole(UserRole.SuperAdmin, UserRole.Student, UserRole.Admin));
        Assert.True(AdminAccessChecker.CanChangeUserRole(UserRole.SuperAdmin, UserRole.Admin, UserRole.SuperAdmin));
        Assert.True(AdminAccessChecker.CanChangeUserRole(UserRole.SuperAdmin, UserRole.SuperAdmin, UserRole.Student));
    }

    [Fact]
    public void CanChangeUserRole_AdminChangingSuperAdmin_ReturnsFalse()
    {
        // Act & Assert
        Assert.False(AdminAccessChecker.CanChangeUserRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Student));
        Assert.False(AdminAccessChecker.CanChangeUserRole(UserRole.Admin, UserRole.Student, UserRole.SuperAdmin));
    }

    [Fact]
    public void CanChangeUserRole_NonAdminUser_ReturnsFalse()
    {
        // Act & Assert
        Assert.False(AdminAccessChecker.CanChangeUserRole(UserRole.Student, UserRole.Student, UserRole.Instructor));
        Assert.False(AdminAccessChecker.CanChangeUserRole(UserRole.Instructor, UserRole.Student, UserRole.Admin));
    }
}
