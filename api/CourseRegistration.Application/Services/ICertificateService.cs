using CourseRegistration.Application.DTOs;

namespace CourseRegistration.Application.Services;

/// <summary>
/// Interface for certificate services
/// </summary>
public interface ICertificateService
{
    /// <summary>
    /// Get all certificates for a student
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <returns>List of certificates</returns>
    Task<IEnumerable<CertificateDto>> GetCertificatesByStudentIdAsync(Guid studentId);

    /// <summary>
    /// Get certificate by certificate ID
    /// </summary>
    /// <param name="certificateId">Certificate ID</param>
    /// <returns>Certificate details</returns>
    Task<CertificateDto?> GetCertificateByIdAsync(Guid certificateId);

    /// <summary>
    /// Get certificates by student name
    /// </summary>
    /// <param name="studentName">Student name</param>
    /// <returns>List of certificates</returns>
    Task<IEnumerable<CertificateDto>> GetCertificatesByStudentNameAsync(string studentName);

    /// <summary>
    /// Get certificates by course ID
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>List of certificates</returns>
    Task<IEnumerable<CertificateDto>> GetCertificatesByCourseIdAsync(Guid courseId);

    /// <summary>
    /// Get certificate by certificate number
    /// </summary>
    /// <param name="certificateNumber">Certificate number</param>
    /// <returns>Certificate details</returns>
    Task<CertificateDto?> GetCertificateByCertificateNumberAsync(string certificateNumber);

    /// <summary>
    /// Create a new certificate
    /// </summary>
    /// <param name="createCertificateDto">Certificate creation data</param>
    /// <returns>Created certificate</returns>
    Task<CertificateDto> CreateCertificateAsync(CreateCertificateDto createCertificateDto);

    /// <summary>
    /// Generate certificate number
    /// </summary>
    /// <returns>Unique certificate number</returns>
    string GenerateCertificateNumber();
}