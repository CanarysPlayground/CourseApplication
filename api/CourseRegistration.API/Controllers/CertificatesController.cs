using Microsoft.AspNetCore.Mvc;
using CourseRegistration.Application.DTOs;
using CourseRegistration.Application.Services;

namespace CourseRegistration.API.Controllers;

/// <summary>
/// Controller for certificate operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CertificatesController : ControllerBase
{
    private readonly ICertificateService _certificateService;
    private readonly ILogger<CertificatesController> _logger;

    /// <summary>
    /// Initializes a new instance of the CertificatesController
    /// </summary>
    public CertificatesController(ICertificateService certificateService, ILogger<CertificatesController> logger)
    {
        _certificateService = certificateService ?? throw new ArgumentNullException(nameof(certificateService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a certificate by ID
    /// </summary>
    /// <param name="id">Certificate ID</param>
    /// <returns>Certificate details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CertificateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CertificateDto>> GetCertificate(Guid id)
    {
        _logger.LogInformation("Getting certificate with ID: {CertificateId}", id);

        var certificate = await _certificateService.GetCertificateByIdAsync(id);
        if (certificate == null)
        {
            return NotFound(new { message = "Certificate not found" });
        }

        return Ok(certificate);
    }

    /// <summary>
    /// Searches certificates by student name
    /// </summary>
    /// <param name="studentName">Student name to search for</param>
    /// <returns>List of matching certificates</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<CertificateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<CertificateDto>>> SearchCertificates(
        [FromQuery] string studentName = "")
    {
        _logger.LogInformation("Searching certificates for student name: {StudentName}", studentName);

        if (string.IsNullOrWhiteSpace(studentName))
        {
            return BadRequest(new { message = "Student name is required" });
        }

        var certificates = await _certificateService.GetCertificatesByStudentNameAsync(studentName);
        return Ok(certificates);
    }

    /// <summary>
    /// Downloads a certificate as a PDF file
    /// </summary>
    /// <param name="certificateId">Certificate ID</param>
    /// <returns>PDF file</returns>
    [HttpGet("{certificateId:guid}/download")]
    [Produces("application/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DownloadCertificate(Guid certificateId)
    {
        _logger.LogInformation("Download requested for certificate ID: {CertificateId}", certificateId);

        if (certificateId == Guid.Empty)
        {
            return BadRequest(new { message = "Invalid certificate ID" });
        }

        var pdfBytes = await _certificateService.DownloadCertificatePdfAsync(certificateId);
        if (pdfBytes == null)
        {
            return NotFound(new { message = "Certificate not found" });
        }

        var fileName = $"certificate-{certificateId}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }
}
