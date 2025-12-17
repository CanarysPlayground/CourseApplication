using Microsoft.AspNetCore.Mvc;
using CourseRegistration.Application.Services;
using CourseRegistration.Application.DTOs;

namespace CourseRegistration.API.Controllers;

/// <summary>
/// API controller for certificate operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CertificatesController : ControllerBase
{
    private readonly ICertificateService _certificateService;
    private readonly ILogger<CertificatesController> _logger;

    public CertificatesController(ICertificateService certificateService, ILogger<CertificatesController> logger)
    {
        _certificateService = certificateService;
        _logger = logger;
    }

    /// <summary>
    /// Get certificate by ID
    /// </summary>
    /// <param name="id">Certificate ID</param>
    /// <returns>Certificate details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CertificateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CertificateDto>> GetCertificateById(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting certificate with ID: {CertificateId}", id);
            var certificate = await _certificateService.GetCertificateByIdAsync(id);
            
            if (certificate == null)
            {
                _logger.LogWarning("Certificate not found: {CertificateId}", id);
                return NotFound(new { message = $"Certificate with ID {id} not found" });
            }
            
            return Ok(certificate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving certificate {CertificateId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving the certificate" });
        }
    }

    /// <summary>
    /// Get certificates by student ID
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <returns>List of certificates for the student</returns>
    [HttpGet("student/{studentId}")]
    [ProducesResponseType(typeof(IEnumerable<CertificateDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CertificateDto>>> GetCertificatesByStudentId(Guid studentId)
    {
        try
        {
            _logger.LogInformation("Getting certificates for student: {StudentId}", studentId);
            var certificates = await _certificateService.GetCertificatesByStudentIdAsync(studentId);
            return Ok(certificates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving certificates for student {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving certificates" });
        }
    }

    /// <summary>
    /// Search certificates by student name
    /// </summary>
    /// <param name="studentName">Student name (partial match supported)</param>
    /// <returns>List of matching certificates</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<CertificateDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CertificateDto>>> SearchCertificates([FromQuery] string studentName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(studentName))
            {
                return BadRequest(new { message = "Student name is required" });
            }

            _logger.LogInformation("Searching certificates for student name: {StudentName}", studentName);
            var certificates = await _certificateService.GetCertificatesByStudentNameAsync(studentName);
            return Ok(certificates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching certificates for student name {StudentName}", studentName);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while searching certificates" });
        }
    }

    /// <summary>
    /// Create a new certificate
    /// </summary>
    /// <param name="createCertificateDto">Certificate creation data</param>
    /// <returns>Created certificate</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CertificateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CertificateDto>> CreateCertificate([FromBody] CreateCertificateDto createCertificateDto)
    {
        try
        {
            if (createCertificateDto == null)
            {
                return BadRequest(new { message = "Certificate data is required" });
            }

            _logger.LogInformation("Creating certificate for student {StudentId} and course {CourseId}", 
                createCertificateDto.StudentId, createCertificateDto.CourseId);
            
            var certificate = await _certificateService.CreateCertificateAsync(createCertificateDto);
            
            return CreatedAtAction(
                nameof(GetCertificateById),
                new { id = certificate.CertificateId },
                certificate
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating certificate");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while creating the certificate" });
        }
    }
}
