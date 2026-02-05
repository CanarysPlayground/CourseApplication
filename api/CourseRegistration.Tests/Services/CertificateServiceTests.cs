using Xunit;
using CourseRegistration.Application.Services;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Tests.Services;

/// <summary>
/// Unit tests for certificate services
/// </summary>
public class CertificateServiceTests
{
    private readonly ICertificateService _certificateService;

    public CertificateServiceTests()
    {
        _certificateService = new CertificateService();
    }

    [Fact]
    public async Task GetCertificateByIdAsync_WithValidId_ReturnsCertificate()
    {
        // Arrange
        var createDto = new CreateCertificateDto
        {
            StudentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            CourseId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            FinalGrade = Grade.A,
            Remarks = "Test certificate"
        };
        var created = await _certificateService.CreateCertificateAsync(createDto);

        // Act
        var result = await _certificateService.GetCertificateByIdAsync(created.CertificateId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.CertificateId, result.CertificateId);
        Assert.Equal(createDto.StudentId, result.StudentId);
        Assert.Equal(createDto.CourseId, result.CourseId);
    }

    [Fact]
    public async Task GetCertificateByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await _certificateService.GetCertificateByIdAsync(invalidId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCertificatesByStudentIdAsync_ReturnsCorrectCertificates()
    {
        // Arrange
        var studentId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Act
        var result = await _certificateService.GetCertificatesByStudentIdAsync(studentId);

        // Assert
        Assert.NotNull(result);
        var certificates = result.ToList();
        Assert.NotEmpty(certificates);
        Assert.All(certificates, c => Assert.Equal(studentId, c.StudentId));
    }

    [Fact]
    public async Task GetCertificatesByStudentNameAsync_WithValidName_ReturnsCertificates()
    {
        // Arrange
        var studentName = "John";

        // Act
        var result = await _certificateService.GetCertificatesByStudentNameAsync(studentName);

        // Assert
        Assert.NotNull(result);
        var certificates = result.ToList();
        Assert.NotEmpty(certificates);
        Assert.All(certificates, c => Assert.Contains("John", c.StudentName));
    }

    [Fact]
    public async Task GetCertificatesByStudentNameAsync_WithInvalidName_ReturnsEmpty()
    {
        // Arrange
        var studentName = "NonExistentStudent";

        // Act
        var result = await _certificateService.GetCertificatesByStudentNameAsync(studentName);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCertificatesByCourseIdAsync_ReturnsCorrectCertificates()
    {
        // Arrange
        var courseId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        // Act
        var result = await _certificateService.GetCertificatesByCourseIdAsync(courseId);

        // Assert
        Assert.NotNull(result);
        var certificates = result.ToList();
        Assert.NotEmpty(certificates);
        Assert.All(certificates, c => Assert.Equal(courseId, c.CourseId));
    }

    [Fact]
    public async Task GetCertificateByCertificateNumberAsync_WithValidNumber_ReturnsCertificate()
    {
        // Arrange
        var certificateNumber = "CERT-2024-001";

        // Act
        var result = await _certificateService.GetCertificateByCertificateNumberAsync(certificateNumber);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(certificateNumber, result.CertificateNumber);
    }

    [Fact]
    public async Task GetCertificateByCertificateNumberAsync_WithInvalidNumber_ReturnsNull()
    {
        // Arrange
        var certificateNumber = "INVALID-NUMBER";

        // Act
        var result = await _certificateService.GetCertificateByCertificateNumberAsync(certificateNumber);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCertificateAsync_CreatesNewCertificate()
    {
        // Arrange
        var createDto = new CreateCertificateDto
        {
            StudentId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            FinalGrade = Grade.B,
            Remarks = "Good work on all assignments"
        };

        // Act
        var result = await _certificateService.CreateCertificateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.CertificateId);
        Assert.Equal(createDto.StudentId, result.StudentId);
        Assert.Equal(createDto.CourseId, result.CourseId);
        Assert.Equal(createDto.FinalGrade, result.FinalGrade);
        Assert.Equal(createDto.Remarks, result.Remarks);
        Assert.NotNull(result.CertificateNumber);
        Assert.NotNull(result.DigitalSignature);
        Assert.NotNull(result.VerificationUrl);
        Assert.NotNull(result.QRCodeData);
    }

    [Fact]
    public void GenerateCertificateNumber_ReturnsValidFormat()
    {
        // Act
        var certificateNumber = _certificateService.GenerateCertificateNumber();

        // Assert
        Assert.NotNull(certificateNumber);
        Assert.StartsWith("CERT-", certificateNumber);
        Assert.Contains(DateTime.Now.Year.ToString(), certificateNumber);
    }

    [Fact]
    public async Task CreateCertificateAsync_GeneratesUniqueCertificateNumbers()
    {
        // Arrange
        var createDto1 = new CreateCertificateDto
        {
            StudentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            CourseId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            FinalGrade = Grade.A,
            Remarks = "First certificate"
        };
        
        var createDto2 = new CreateCertificateDto
        {
            StudentId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            FinalGrade = Grade.B,
            Remarks = "Second certificate"
        };

        // Act
        var certificate1 = await _certificateService.CreateCertificateAsync(createDto1);
        var certificate2 = await _certificateService.CreateCertificateAsync(createDto2);

        // Assert
        Assert.NotEqual(certificate1.CertificateNumber, certificate2.CertificateNumber);
    }

    [Theory]
    [InlineData(Grade.A)]
    [InlineData(Grade.B)]
    [InlineData(Grade.C)]
    [InlineData(Grade.D)]
    [InlineData(Grade.F)]
    public async Task CreateCertificateAsync_WithDifferentGrades_CreatesSuccessfully(Grade grade)
    {
        // Arrange
        var createDto = new CreateCertificateDto
        {
            StudentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            CourseId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            FinalGrade = grade,
            Remarks = $"Certificate with grade {grade}"
        };

        // Act
        var result = await _certificateService.CreateCertificateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(grade, result.FinalGrade);
    }

    [Fact]
    public async Task Certificate_HasVerificationUrl()
    {
        // Arrange
        var studentId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Act
        var certificates = await _certificateService.GetCertificatesByStudentIdAsync(studentId);
        var certificate = certificates.FirstOrDefault();

        // Assert
        Assert.NotNull(certificate);
        Assert.NotNull(certificate.VerificationUrl);
        Assert.Contains("verify", certificate.VerificationUrl.ToLower());
    }

    [Fact]
    public async Task Certificate_HasQRCodeData()
    {
        // Arrange
        var studentId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Act
        var certificates = await _certificateService.GetCertificatesByStudentIdAsync(studentId);
        var certificate = certificates.FirstOrDefault();

        // Assert
        Assert.NotNull(certificate);
        Assert.NotNull(certificate.QRCodeData);
        Assert.Contains(certificate.CertificateId.ToString(), certificate.QRCodeData);
    }
}
