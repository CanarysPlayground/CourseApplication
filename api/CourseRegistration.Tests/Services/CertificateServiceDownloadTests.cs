using Xunit;
using CourseRegistration.Application.Services;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Tests.Services;

/// <summary>
/// Unit tests for CertificateService PDF download functionality
/// </summary>
public class CertificateServiceDownloadTests
{
    private readonly CertificateService _service;

    public CertificateServiceDownloadTests()
    {
        _service = new CertificateService();
    }

    [Fact]
    public async Task DownloadCertificatePdfAsync_WithValidId_ReturnsPdfBytes()
    {
        // Arrange
        var newCert = await _service.CreateCertificateAsync(new CreateCertificateDto
        {
            StudentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            CourseId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            FinalGrade = Grade.A,
            Remarks = "Test certificate"
        });

        // Act
        var pdfBytes = await _service.DownloadCertificatePdfAsync(newCert.CertificateId);

        // Assert
        Assert.NotNull(pdfBytes);
        Assert.True(pdfBytes.Length > 0);
    }

    [Fact]
    public async Task DownloadCertificatePdfAsync_WithValidId_ReturnsPdfHeader()
    {
        // Arrange
        var newCert = await _service.CreateCertificateAsync(new CreateCertificateDto
        {
            StudentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            CourseId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            FinalGrade = Grade.B,
            Remarks = "PDF header test"
        });

        // Act
        var pdfBytes = await _service.DownloadCertificatePdfAsync(newCert.CertificateId);

        // Assert
        Assert.NotNull(pdfBytes);
        var header = System.Text.Encoding.Latin1.GetString(pdfBytes, 0, Math.Min(8, pdfBytes.Length));
        Assert.StartsWith("%PDF-", header);
    }

    [Fact]
    public async Task DownloadCertificatePdfAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var pdfBytes = await _service.DownloadCertificatePdfAsync(nonExistentId);

        // Assert
        Assert.Null(pdfBytes);
    }

    [Fact]
    public async Task DownloadCertificatePdfAsync_WithEmptyGuid_ReturnsNull()
    {
        // Act
        var pdfBytes = await _service.DownloadCertificatePdfAsync(Guid.Empty);

        // Assert
        Assert.Null(pdfBytes);
    }

    [Fact]
    public async Task DownloadCertificatePdfAsync_PdfContainsCertificateDetails()
    {
        // Arrange
        // StudentId 11111111... maps to "John Doe" and CourseId 33333333... maps to
        // "Introduction to Programming" per CertificateService in-memory seed data.
        var newCert = await _service.CreateCertificateAsync(new CreateCertificateDto
        {
            StudentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            CourseId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            FinalGrade = Grade.A,
            Remarks = "Content verification test"
        });

        // Act
        var pdfBytes = await _service.DownloadCertificatePdfAsync(newCert.CertificateId);

        // Assert
        Assert.NotNull(pdfBytes);
        var pdfContent = System.Text.Encoding.Latin1.GetString(pdfBytes);
        Assert.Contains("Certificate of Completion", pdfContent);
        Assert.Contains("John Doe", pdfContent);
        Assert.Contains("Introduction to Programming", pdfContent);
    }

    [Fact]
    public async Task DownloadCertificatePdfAsync_PdfEndsWithEof()
    {
        // Arrange
        var newCert = await _service.CreateCertificateAsync(new CreateCertificateDto
        {
            StudentId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            FinalGrade = Grade.B,
            Remarks = "EOF test"
        });

        // Act
        var pdfBytes = await _service.DownloadCertificatePdfAsync(newCert.CertificateId);

        // Assert
        Assert.NotNull(pdfBytes);
        var pdfContent = System.Text.Encoding.Latin1.GetString(pdfBytes);
        Assert.Contains("%%EOF", pdfContent);
    }
}
