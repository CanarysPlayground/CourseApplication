using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CourseRegistration.Application.Utilities;

/// <summary>
/// Utility class for certificate signature generation and verification according to Rule 3.2 and 3.5
/// </summary>
public static class CertificateSignatureHelper
{
    /// <summary>
    /// Generates a canonical JSON payload from certificate data
    /// Ensures stable, ordered JSON representation for consistent signature computation
    /// </summary>
    public static string GenerateCanonicalPayload(
        Guid certificateId,
        string serialNumber,
        string holderName,
        string courseTitle,
        DateTime issueDateUtc,
        DateTime? expiryDateUtc,
        string issuedBy,
        string version)
    {
        // Create dictionary with stable ordering (alphabetical by key)
        var payload = new SortedDictionary<string, object?>
        {
            { "certificateId", certificateId.ToString("D").ToLowerInvariant() }, // Canonical GUID format
            { "courseTitle", courseTitle },
            { "expiryDateUtc", expiryDateUtc?.ToString("O") }, // ISO 8601 format
            { "holderName", holderName },
            { "issueDateUtc", issueDateUtc.ToString("O") }, // ISO 8601 format
            { "issuedBy", issuedBy },
            { "serialNumber", serialNumber },
            { "version", version }
        };

        // Serialize with consistent options
        var options = new JsonSerializerOptions
        {
            WriteIndented = false, // No whitespace for consistency
            PropertyNamingPolicy = null // Keep exact key names
        };

        return JsonSerializer.Serialize(payload, options);
    }

    /// <summary>
    /// Computes SHA-256 hash of the canonical payload
    /// </summary>
    public static string ComputeSignatureHash(string canonicalPayload)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(canonicalPayload);
        var hashBytes = sha256.ComputeHash(bytes);
        
        // Convert to lowercase hexadecimal string
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    /// <summary>
    /// Generates signature hash for a certificate
    /// </summary>
    public static string GenerateSignature(
        Guid certificateId,
        string serialNumber,
        string holderName,
        string courseTitle,
        DateTime issueDateUtc,
        DateTime? expiryDateUtc,
        string issuedBy,
        string version)
    {
        var canonicalPayload = GenerateCanonicalPayload(
            certificateId,
            serialNumber,
            holderName,
            courseTitle,
            issueDateUtc,
            expiryDateUtc,
            issuedBy,
            version);

        return ComputeSignatureHash(canonicalPayload);
    }

    /// <summary>
    /// Verifies the signature hash of a certificate
    /// </summary>
    public static bool VerifySignature(
        Guid certificateId,
        string serialNumber,
        string holderName,
        string courseTitle,
        DateTime issueDateUtc,
        DateTime? expiryDateUtc,
        string issuedBy,
        string version,
        string expectedHash)
    {
        var computedHash = GenerateSignature(
            certificateId,
            serialNumber,
            holderName,
            courseTitle,
            issueDateUtc,
            expiryDateUtc,
            issuedBy,
            version);

        // Case-insensitive comparison
        return string.Equals(computedHash, expectedHash, StringComparison.OrdinalIgnoreCase);
    }
}
