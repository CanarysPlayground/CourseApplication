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

    public async Task<byte[]?> DownloadCertificatePdfAsync(Guid certificateId)
    {
        await Task.CompletedTask; // Simulate async operation

        var certificate = _certificates.FirstOrDefault(c => c.CertificateId == certificateId);
        if (certificate == null)
        {
            return null;
        }

        var dto = MapToDto(certificate);
        return BuildMinimalPdf(dto);
    }

    private static byte[] BuildMinimalPdf(CertificateDto cert)
    {
        // Build the page content stream using standard PDF Type1 font (Helvetica)
        var content = new System.Text.StringBuilder();
        content.AppendLine("BT");
        content.AppendLine("/F1 28 Tf");
        content.AppendLine("160 720 Td");
        content.AppendLine($"(Certificate of Completion) Tj");
        content.AppendLine("/F1 18 Tf");
        content.AppendLine("0 -60 Td");
        content.AppendLine($"(This certifies that) Tj");
        content.AppendLine("/F1 22 Tf");
        content.AppendLine("0 -40 Td");
        var displayName = cert.StudentName.Length > 60 ? cert.StudentName[..60] + "..." : cert.StudentName;
        content.AppendLine($"({EscapePdfString(displayName)}) Tj");
        content.AppendLine("/F1 16 Tf");
        content.AppendLine("0 -40 Td");
        content.AppendLine("(has successfully completed the course:) Tj");
        content.AppendLine("/F1 20 Tf");
        content.AppendLine("0 -36 Td");
        content.AppendLine($"({EscapePdfString(cert.CourseName)}) Tj");
        content.AppendLine("/F1 14 Tf");
        content.AppendLine("0 -50 Td");
        content.AppendLine($"(Instructor: {EscapePdfString(cert.InstructorName)}) Tj");
        content.AppendLine("0 -24 Td");
        content.AppendLine($"(Issue Date: {cert.IssueDate.ToUniversalTime():yyyy-MM-ddTHH:mm:ssZ}) Tj");
        content.AppendLine("0 -24 Td");
        content.AppendLine($"(Certificate No: {EscapePdfString(cert.CertificateNumber)}) Tj");
        content.AppendLine("0 -24 Td");
        content.AppendLine($"(Grade: {cert.FinalGrade}) Tj");
        content.AppendLine("ET");

        var streamBytes = System.Text.Encoding.Latin1.GetBytes(content.ToString());
        var streamLength = streamBytes.Length;

        // Assemble PDF objects
        var objects = new List<string>();

        // Object 1: Catalog
        objects.Add("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj");
        // Object 2: Pages
        objects.Add("2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj");
        // Object 3: Page
        objects.Add("3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792]\n   /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>\nendobj");
        // Object 4: Content stream
        objects.Add($"4 0 obj\n<< /Length {streamLength} >>\nstream\n{content}endstream\nendobj");
        // Object 5: Font
        objects.Add("5 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>\nendobj");

        // Build xref table
        var pdfBuilder = new System.Text.StringBuilder();
        pdfBuilder.Append("%PDF-1.4\n");

        var offsets = new List<int>();
        foreach (var obj in objects)
        {
            offsets.Add(pdfBuilder.Length);
            pdfBuilder.Append(obj);
            pdfBuilder.Append("\n");
        }

        var xrefOffset = pdfBuilder.Length;
        pdfBuilder.Append("xref\n");
        pdfBuilder.Append($"0 {objects.Count + 1}\n");
        pdfBuilder.Append("0000000000 65535 f \n");
        foreach (var offset in offsets)
        {
            pdfBuilder.Append($"{offset:D10} 00000 n \n");
        }
        pdfBuilder.Append("trailer\n");
        pdfBuilder.Append($"<< /Size {objects.Count + 1} /Root 1 0 R >>\n");
        pdfBuilder.Append("startxref\n");
        pdfBuilder.Append($"{xrefOffset}\n");
        pdfBuilder.Append("%%EOF\n");

        return System.Text.Encoding.Latin1.GetBytes(pdfBuilder.ToString());
    }

    private static string EscapePdfString(string input)
    {
        // Filter to Latin1 (ISO-8859-1) range, then escape special PDF string characters
        var latin1Safe = new System.Text.StringBuilder(input.Length);
        foreach (var ch in input)
        {
            if (ch <= 0xFF)
            {
                latin1Safe.Append(ch);
            }
            else
            {
                latin1Safe.Append('?');
            }
        }

        return latin1Safe.ToString()
            .Replace("\\", "\\\\")
            .Replace("(", "\\(")
            .Replace(")", "\\)")
            .Replace("\r", "")
            .Replace("\n", " ");
    }

    private CertificateDto MapToDto(Certificate certificate)
    {
        var student = _students.FirstOrDefault(s => s.StudentId == certificate.StudentId);
        var course = _courses.FirstOrDefault(c => c.CourseId == certificate.CourseId);

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
            DigitalSignature = certificate.DigitalSignature
        };
    }
}