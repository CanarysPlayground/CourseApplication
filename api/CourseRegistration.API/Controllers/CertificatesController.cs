using Microsoft.AspNetCore.Mvc;
using CourseRegistration.Application.Services;
using CourseRegistration.Application.DTOs;
using Serilog;

namespace CourseRegistration.API.Controllers;

/// <summary>
/// Controller for managing certificates
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CertificatesController : ControllerBase
{
    private readonly ICertificateService _certificateService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="certificateService">Certificate service</param>
    public CertificatesController(ICertificateService certificateService)
    {
        _certificateService = certificateService;
    }

    /// <summary>
    /// Search for certificates by various parameters
    /// </summary>
    /// <param name="studentId">Optional student ID</param>
    /// <param name="studentName">Optional student name</param>
    /// <param name="courseId">Optional course ID</param>
    /// <param name="certificateNumber">Optional certificate number</param>
    /// <returns>List of certificates matching the search criteria</returns>
    /// <response code="200">Returns the list of certificates</response>
    /// <response code="400">If the request parameters are invalid</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<CertificateDto>>> SearchCertificates(
        [FromQuery] Guid? studentId = null,
        [FromQuery] string? studentName = null,
        [FromQuery] Guid? courseId = null,
        [FromQuery] string? certificateNumber = null)
    {
        try
        {
            // Search by certificate number first (most specific)
            if (!string.IsNullOrWhiteSpace(certificateNumber))
            {
                var certificate = await _certificateService.GetCertificateByCertificateNumberAsync(certificateNumber);
                return Ok(certificate != null ? new[] { certificate } : Array.Empty<CertificateDto>());
            }

            // Search by student ID
            if (studentId.HasValue)
            {
                var certificates = await _certificateService.GetCertificatesByStudentIdAsync(studentId.Value);
                return Ok(certificates);
            }

            // Search by student name
            if (!string.IsNullOrWhiteSpace(studentName))
            {
                var certificates = await _certificateService.GetCertificatesByStudentNameAsync(studentName);
                return Ok(certificates);
            }

            // Search by course ID
            if (courseId.HasValue)
            {
                var certificates = await _certificateService.GetCertificatesByCourseIdAsync(courseId.Value);
                return Ok(certificates);
            }

            // If no search parameters provided, return empty list
            Log.Warning("Certificate search called without any search parameters");
            return Ok(Array.Empty<CertificateDto>());
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error searching for certificates");
            return StatusCode(500, new { message = "An error occurred while searching for certificates" });
        }
    }

    /// <summary>
    /// Get a specific certificate by ID
    /// </summary>
    /// <param name="id">Certificate ID</param>
    /// <returns>Certificate details</returns>
    /// <response code="200">Returns the certificate</response>
    /// <response code="404">If the certificate is not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CertificateDto>> GetCertificate(Guid id)
    {
        try
        {
            var certificate = await _certificateService.GetCertificateByIdAsync(id);

            if (certificate == null)
            {
                Log.Information("Certificate not found: {CertificateId}", id);
                return NotFound(new { message = $"Certificate with ID {id} not found" });
            }

            return Ok(certificate);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving certificate {CertificateId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the certificate" });
        }
    }

    /// <summary>
    /// Get certificates for a specific student
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <returns>List of certificates for the student</returns>
    /// <response code="200">Returns the list of certificates</response>
    [HttpGet("student/{studentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CertificateDto>>> GetCertificatesByStudent(Guid studentId)
    {
        try
        {
            var certificates = await _certificateService.GetCertificatesByStudentIdAsync(studentId);
            return Ok(certificates);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving certificates for student {StudentId}", studentId);
            return StatusCode(500, new { message = "An error occurred while retrieving certificates" });
        }
    }

    /// <summary>
    /// Get certificates for a specific course
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>List of certificates for the course</returns>
    /// <response code="200">Returns the list of certificates</response>
    [HttpGet("course/{courseId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CertificateDto>>> GetCertificatesByCourse(Guid courseId)
    {
        try
        {
            var certificates = await _certificateService.GetCertificatesByCourseIdAsync(courseId);
            return Ok(certificates);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving certificates for course {CourseId}", courseId);
            return StatusCode(500, new { message = "An error occurred while retrieving certificates" });
        }
    }

    /// <summary>
    /// Verify a certificate by its ID or certificate number
    /// </summary>
    /// <param name="identifier">Certificate ID or certificate number</param>
    /// <returns>Certificate verification result</returns>
    /// <response code="200">Returns the certificate if valid</response>
    /// <response code="404">If the certificate is not found</response>
    [HttpGet("verify/{identifier}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CertificateDto>> VerifyCertificate(string identifier)
    {
        try
        {
            // Try to parse as GUID first
            if (Guid.TryParse(identifier, out Guid certificateId))
            {
                var certificate = await _certificateService.GetCertificateByIdAsync(certificateId);
                if (certificate != null)
                {
                    return Ok(certificate);
                }
            }

            // Try as certificate number
            var certByNumber = await _certificateService.GetCertificateByCertificateNumberAsync(identifier);
            if (certByNumber != null)
            {
                return Ok(certByNumber);
            }

            Log.Information("Certificate verification failed for identifier: {Identifier}", identifier);
            return NotFound(new { message = "Certificate not found or invalid" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error verifying certificate {Identifier}", identifier);
            return StatusCode(500, new { message = "An error occurred while verifying the certificate" });
        }
    }

    /// <summary>
    /// Create a new certificate for a completed course
    /// </summary>
    /// <param name="createCertificateDto">Certificate creation data</param>
    /// <returns>The created certificate</returns>
    /// <response code="201">Returns the created certificate</response>
    /// <response code="400">If the request data is invalid</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CertificateDto>> CreateCertificate([FromBody] CreateCertificateDto createCertificateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var certificate = await _certificateService.CreateCertificateAsync(createCertificateDto);
            
            Log.Information("Certificate created: {CertificateId} for student {StudentId}", 
                certificate.CertificateId, certificate.StudentId);

            return CreatedAtAction(
                nameof(GetCertificate),
                new { id = certificate.CertificateId },
                certificate);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating certificate");
            return StatusCode(500, new { message = "An error occurred while creating the certificate" });
        }
    }
}
