using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Application.DTOs;

/// <summary>
/// Data transfer object for certificate information
/// </summary>
public class CertificateDto
{
    public Guid CertificateId { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime IssueDate { get; set; }
    public Grade FinalGrade { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public DateTime CourseStartDate { get; set; }
    public DateTime CourseEndDate { get; set; }
    public string? Remarks { get; set; }
    public string? DigitalSignature { get; set; }
}

/// <summary>
/// Data transfer object for creating a new certificate
/// </summary>
public class CreateCertificateDto
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public Grade FinalGrade { get; set; }
    public string? Remarks { get; set; }
}

/// <summary>
/// Data transfer object for certificate request by student name
/// </summary>
public class CertificateRequestDto
{
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
}