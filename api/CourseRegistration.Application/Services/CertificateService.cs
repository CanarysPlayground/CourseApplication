using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Services;
using CourseRegistration.Domain.Entities;
using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Application.Services;

/// <summary>
/// Implementation of certificate services
/// </summary>
public class CertificateService : ICertificateService
{
    // In a real application, this would use a repository pattern with Entity Framework
    // For demo purposes, I'll use in-memory data
    private static readonly List<Certificate> _certificates = new();
    private static readonly List<Student> _students = new()
    {
        new Student
        {
            StudentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            DateOfBirth = new DateTime(1995, 5, 15)
        },
        new Student
        {
            StudentId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            DateOfBirth = new DateTime(1996, 8, 20)
        }
    };

    private static readonly List<Course> _courses = new()
    {
        new Course
        {
            CourseId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            CourseName = "Introduction to Programming",
            Description = "Learn the basics of programming",
            InstructorName = "Dr. Alan Turing",
            StartDate = new DateTime(2024, 1, 15),
            EndDate = new DateTime(2024, 5, 15),
            Schedule = "MWF 10:00-11:00"
        },
        new Course
        {
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            CourseName = "Web Development",
            Description = "Learn modern web development",
            InstructorName = "Prof. Tim Berners-Lee",
            StartDate = new DateTime(2024, 2, 1),
            EndDate = new DateTime(2024, 6, 1),
            Schedule = "TTh 14:00-16:00"
        }
    };

    static CertificateService()
    {
        // Initialize with some sample certificates
        _certificates.AddRange(new[]
        {
            new Certificate
            {
                CertificateId = Guid.NewGuid(),
                StudentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CourseId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                IssueDate = DateTime.UtcNow.AddDays(-30),
                FinalGrade = Grade.A,
                CertificateNumber = "CERT-2024-001",
                Remarks = "Outstanding performance",
                DigitalSignature = "DS-" + Guid.NewGuid().ToString()[..8]
            },
            new Certificate
            {
                CertificateId = Guid.NewGuid(),
                StudentId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                IssueDate = DateTime.UtcNow.AddDays(-15),
                FinalGrade = Grade.B,
                CertificateNumber = "CERT-2024-002",
                Remarks = "Good work",
                DigitalSignature = "DS-" + Guid.NewGuid().ToString()[..8]
            }
        });
    }

    public async Task<IEnumerable<CertificateDto>> GetCertificatesByStudentIdAsync(Guid studentId)
    {
        await Task.CompletedTask; // Simulate async operation
        
        var certificates = _certificates.Where(c => c.StudentId == studentId);
        return certificates.Select(MapToDto);
    }

    public async Task<CertificateDto?> GetCertificateByIdAsync(Guid certificateId)
    {
        await Task.CompletedTask; // Simulate async operation
        
        var certificate = _certificates.FirstOrDefault(c => c.CertificateId == certificateId);
        return certificate != null ? MapToDto(certificate) : null;
    }

    public async Task<IEnumerable<CertificateDto>> GetCertificatesByStudentNameAsync(string studentName)
    {
        await Task.CompletedTask; // Simulate async operation
        
        var matchingStudents = _students.Where(s => 
            $"{s.FirstName} {s.LastName}".Contains(studentName, StringComparison.OrdinalIgnoreCase) ||
            s.FirstName.Contains(studentName, StringComparison.OrdinalIgnoreCase) ||
            s.LastName.Contains(studentName, StringComparison.OrdinalIgnoreCase));

        var certificates = new List<Certificate>();
        foreach (var student in matchingStudents)
        {
            certificates.AddRange(_certificates.Where(c => c.StudentId == student.StudentId));
        }

        return certificates.Select(MapToDto);
    }

    public async Task<IEnumerable<CertificateDto>> GetCertificatesByCourseIdAsync(Guid courseId)
    {
        await Task.CompletedTask; // Simulate async operation
        
        var certificates = _certificates.Where(c => c.CourseId == courseId);
        return certificates.Select(MapToDto);
    }

    public async Task<CertificateDto?> GetCertificateByCertificateNumberAsync(string certificateNumber)
    {
        await Task.CompletedTask; // Simulate async operation
        
        var certificate = _certificates.FirstOrDefault(c => 
            c.CertificateNumber.Equals(certificateNumber, StringComparison.OrdinalIgnoreCase));
        return certificate != null ? MapToDto(certificate) : null;
    }

    public async Task<CertificateDto> CreateCertificateAsync(CreateCertificateDto createCertificateDto)
    {
        await Task.CompletedTask; // Simulate async operation
        
        var certificate = new Certificate
        {
            CertificateId = Guid.NewGuid(),
            StudentId = createCertificateDto.StudentId,
            CourseId = createCertificateDto.CourseId,
            IssueDate = DateTime.UtcNow,
            FinalGrade = createCertificateDto.FinalGrade,
            CertificateNumber = GenerateCertificateNumber(),
            Remarks = createCertificateDto.Remarks,
            DigitalSignature = "DS-" + Guid.NewGuid().ToString()[..8]
        };

        _certificates.Add(certificate);
        return MapToDto(certificate);
    }

    public string GenerateCertificateNumber()
    {
        var year = DateTime.Now.Year;
        var sequence = _certificates.Count + 1;
        return $"CERT-{year}-{sequence:D3}";
    }

    private CertificateDto MapToDto(Certificate certificate)
    {
        var student = _students.FirstOrDefault(s => s.StudentId == certificate.StudentId);
        var course = _courses.FirstOrDefault(c => c.CourseId == certificate.CourseId);

        // Generate verification URL and QR code data
        var verificationUrl = $"https://courseregistration.app/api/certificates/verify/{certificate.CertificateNumber}";
        var qrCodeData = $"{verificationUrl}|{certificate.CertificateId}|{certificate.DigitalSignature}";

        return new CertificateDto
        {
            CertificateId = certificate.CertificateId,
            StudentId = certificate.StudentId,
            CourseId = certificate.CourseId,
            IssueDate = certificate.IssueDate,
            FinalGrade = certificate.FinalGrade,
            CertificateNumber = certificate.CertificateNumber,
            StudentName = student != null ? $"{student.FirstName} {student.LastName}" : "Unknown Student",
            CourseName = course?.CourseName ?? "Unknown Course",
            InstructorName = course?.InstructorName ?? "Unknown Instructor",
            CourseStartDate = course?.StartDate ?? DateTime.MinValue,
            CourseEndDate = course?.EndDate ?? DateTime.MinValue,
            Remarks = certificate.Remarks,
            DigitalSignature = certificate.DigitalSignature,
            VerificationUrl = verificationUrl,
            QRCodeData = qrCodeData
        };
    }
}