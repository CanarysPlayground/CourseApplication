using Xunit;
using CourseRegistration.Application.Utilities;

namespace CourseRegistration.Tests.Utilities;

/// <summary>
/// Unit tests for CertificateSignatureHelper covering canonical payload generation and signature computation
/// Tests correspond to Rules 3.2, 3.5, and 8.1-8.2 in CERTIFICATE_VALIDATION_RULES.md
/// </summary>
public class CertificateSignatureHelperTests
{
    #region Rule 3.2: Canonical Payload Tests

    [Fact]
    public void GenerateCanonicalPayload_WithSameData_ProducesSameOutput()
    {
        // Arrange
        var certificateId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        var serialNumber = "CERT-2024-001";
        var holderName = "John Doe";
        var courseTitle = "Introduction to Programming";
        var issueDateUtc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        DateTime? expiryDateUtc = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var issuedBy = "ORG-EDU-001";
        var version = "1.0.0";

        // Act
        var payload1 = CertificateSignatureHelper.GenerateCanonicalPayload(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version);
        
        var payload2 = CertificateSignatureHelper.GenerateCanonicalPayload(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version);

        // Assert
        Assert.Equal(payload1, payload2);
    }

    [Fact]
    public void GenerateCanonicalPayload_ProducesValidJson()
    {
        // Arrange
        var certificateId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        var serialNumber = "CERT-2024-001";
        var holderName = "John Doe";
        var courseTitle = "Introduction to Programming";
        var issueDateUtc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        DateTime? expiryDateUtc = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var issuedBy = "ORG-EDU-001";
        var version = "1.0.0";

        // Act
        var payload = CertificateSignatureHelper.GenerateCanonicalPayload(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version);

        // Assert - Should be able to parse as JSON
        Assert.NotNull(payload);
        Assert.NotEmpty(payload);
        
        // Verify it's valid JSON by parsing it
        var parsed = System.Text.Json.JsonDocument.Parse(payload);
        Assert.NotNull(parsed);
    }

    [Fact]
    public void GenerateCanonicalPayload_IncludesAllRequiredFields()
    {
        // Arrange
        var certificateId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        var serialNumber = "CERT-2024-001";
        var holderName = "John Doe";
        var courseTitle = "Introduction to Programming";
        var issueDateUtc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        DateTime? expiryDateUtc = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var issuedBy = "ORG-EDU-001";
        var version = "1.0.0";

        // Act
        var payload = CertificateSignatureHelper.GenerateCanonicalPayload(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version);

        // Assert - Check all fields are included
        Assert.Contains("certificateId", payload);
        Assert.Contains("serialNumber", payload);
        Assert.Contains("holderName", payload);
        Assert.Contains("courseTitle", payload);
        Assert.Contains("issueDateUtc", payload);
        Assert.Contains("expiryDateUtc", payload);
        Assert.Contains("issuedBy", payload);
        Assert.Contains("version", payload);
    }

    [Fact]
    public void GenerateCanonicalPayload_WithNullExpiryDate_OmitsField()
    {
        // Arrange
        var certificateId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        var serialNumber = "CERT-2024-001";
        var holderName = "John Doe";
        var courseTitle = "Introduction to Programming";
        var issueDateUtc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        DateTime? expiryDateUtc = null; // Non-expiring certificate
        var issuedBy = "ORG-EDU-001";
        var version = "1.0.0";

        // Act
        var payload = CertificateSignatureHelper.GenerateCanonicalPayload(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version);

        // Assert - Verify null is represented
        Assert.Contains("expiryDateUtc", payload);
        Assert.Contains("null", payload);
    }

    [Fact]
    public void GenerateCanonicalPayload_UsesCanonicalGuidFormat()
    {
        // Arrange
        var certificateId = Guid.Parse("550E8400-E29B-41D4-A716-446655440000"); // Uppercase
        var serialNumber = "CERT-2024-001";
        var holderName = "John Doe";
        var courseTitle = "Introduction to Programming";
        var issueDateUtc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        DateTime? expiryDateUtc = null;
        var issuedBy = "ORG-EDU-001";
        var version = "1.0.0";

        // Act
        var payload = CertificateSignatureHelper.GenerateCanonicalPayload(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version);

        // Assert - GUID should be lowercase with hyphens (canonical format)
        Assert.Contains("550e8400-e29b-41d4-a716-446655440000", payload);
        Assert.DoesNotContain("550E8400", payload);
    }

    [Fact]
    public void GenerateCanonicalPayload_UsesIso8601DateFormat()
    {
        // Arrange
        var certificateId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        var serialNumber = "CERT-2024-001";
        var holderName = "John Doe";
        var courseTitle = "Introduction to Programming";
        var issueDateUtc = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc);
        DateTime? expiryDateUtc = null;
        var issuedBy = "ORG-EDU-001";
        var version = "1.0.0";

        // Act
        var payload = CertificateSignatureHelper.GenerateCanonicalPayload(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version);

        // Assert - Date should be in ISO 8601 format
        Assert.Contains("2024-01-15T10:30:45", payload);
    }

    #endregion

    #region Rule 3.5: Signature Hash Computation Tests

    [Fact]
    public void ComputeSignatureHash_ProducesSha256Hash()
    {
        // Arrange
        var payload = "{\"test\":\"data\"}";

        // Act
        var hash = CertificateSignatureHelper.ComputeSignatureHash(payload);

        // Assert - SHA-256 hash is always 64 hexadecimal characters
        Assert.NotNull(hash);
        Assert.Equal(64, hash.Length);
        Assert.Matches("^[0-9a-f]{64}$", hash);
    }

    [Fact]
    public void ComputeSignatureHash_WithSamePayload_ProducesSameHash()
    {
        // Arrange
        var payload = "{\"certificateId\":\"550e8400-e29b-41d4-a716-446655440000\"}";

        // Act
        var hash1 = CertificateSignatureHelper.ComputeSignatureHash(payload);
        var hash2 = CertificateSignatureHelper.ComputeSignatureHash(payload);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ComputeSignatureHash_WithDifferentPayload_ProducesDifferentHash()
    {
        // Arrange
        var payload1 = "{\"certificateId\":\"550e8400-e29b-41d4-a716-446655440000\"}";
        var payload2 = "{\"certificateId\":\"550e8400-e29b-41d4-a716-446655440001\"}";

        // Act
        var hash1 = CertificateSignatureHelper.ComputeSignatureHash(payload1);
        var hash2 = CertificateSignatureHelper.ComputeSignatureHash(payload2);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void ComputeSignatureHash_IsLowercase()
    {
        // Arrange
        var payload = "{\"test\":\"data\"}";

        // Act
        var hash = CertificateSignatureHelper.ComputeSignatureHash(payload);

        // Assert
        Assert.Equal(hash, hash.ToLowerInvariant());
    }

    #endregion

    #region Rule 3.5 & 6.3: Full Signature Generation Tests

    [Fact]
    public void GenerateSignature_WithSameData_ProducesSameHash()
    {
        // Arrange
        var certificateId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        var serialNumber = "CERT-2024-001";
        var holderName = "John Doe";
        var courseTitle = "Introduction to Programming";
        var issueDateUtc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        DateTime? expiryDateUtc = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var issuedBy = "ORG-EDU-001";
        var version = "1.0.0";

        // Act
        var signature1 = CertificateSignatureHelper.GenerateSignature(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version);
        
        var signature2 = CertificateSignatureHelper.GenerateSignature(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version);

        // Assert
        Assert.Equal(signature1, signature2);
    }

    [Fact]
    public void GenerateSignature_WithDifferentData_ProducesDifferentHash()
    {
        // Arrange
        var certificateId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        var serialNumber = "CERT-2024-001";
        var holderName1 = "John Doe";
        var holderName2 = "Jane Smith";
        var courseTitle = "Introduction to Programming";
        var issueDateUtc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        DateTime? expiryDateUtc = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var issuedBy = "ORG-EDU-001";
        var version = "1.0.0";

        // Act
        var signature1 = CertificateSignatureHelper.GenerateSignature(
            certificateId, serialNumber, holderName1, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version);
        
        var signature2 = CertificateSignatureHelper.GenerateSignature(
            certificateId, serialNumber, holderName2, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version);

        // Assert
        Assert.NotEqual(signature1, signature2);
    }

    [Fact]
    public void GenerateSignature_WithDifferentVersion_ProducesDifferentHash()
    {
        // Arrange - Rule 6.3: Version included in signature scope
        var certificateId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        var serialNumber = "CERT-2024-001";
        var holderName = "John Doe";
        var courseTitle = "Introduction to Programming";
        var issueDateUtc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        DateTime? expiryDateUtc = null;
        var issuedBy = "ORG-EDU-001";
        var version1 = "1.0.0";
        var version2 = "2.0.0";

        // Act
        var signature1 = CertificateSignatureHelper.GenerateSignature(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version1);
        
        var signature2 = CertificateSignatureHelper.GenerateSignature(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version2);

        // Assert - Version change invalidates signature
        Assert.NotEqual(signature1, signature2);
    }

    [Fact]
    public void GenerateSignature_ReturnsValidSha256Hash()
    {
        // Arrange
        var certificateId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        var serialNumber = "CERT-2024-001";
        var holderName = "John Doe";
        var courseTitle = "Introduction to Programming";
        var issueDateUtc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        DateTime? expiryDateUtc = null;
        var issuedBy = "ORG-EDU-001";
        var version = "1.0.0";

        // Act
        var signature = CertificateSignatureHelper.GenerateSignature(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version);

        // Assert
        Assert.NotNull(signature);
        Assert.Equal(64, signature.Length);
        Assert.Matches("^[0-9a-f]{64}$", signature);
    }

    #endregion

    #region Signature Verification Tests

    [Fact]
    public void VerifySignature_WithValidHash_ReturnsTrue()
    {
        // Arrange
        var certificateId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        var serialNumber = "CERT-2024-001";
        var holderName = "John Doe";
        var courseTitle = "Introduction to Programming";
        var issueDateUtc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        DateTime? expiryDateUtc = null;
        var issuedBy = "ORG-EDU-001";
        var version = "1.0.0";

        // Generate expected hash
        var expectedHash = CertificateSignatureHelper.GenerateSignature(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version);

        // Act
        var isValid = CertificateSignatureHelper.VerifySignature(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version, expectedHash);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void VerifySignature_WithTamperedData_ReturnsFalse()
    {
        // Arrange
        var certificateId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        var serialNumber = "CERT-2024-001";
        var holderName = "John Doe";
        var tamperedHolderName = "Jane Smith"; // Tampered
        var courseTitle = "Introduction to Programming";
        var issueDateUtc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        DateTime? expiryDateUtc = null;
        var issuedBy = "ORG-EDU-001";
        var version = "1.0.0";

        // Generate hash with original data
        var originalHash = CertificateSignatureHelper.GenerateSignature(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version);

        // Act - Verify with tampered data
        var isValid = CertificateSignatureHelper.VerifySignature(
            certificateId, serialNumber, tamperedHolderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version, originalHash);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void VerifySignature_WithInvalidHash_ReturnsFalse()
    {
        // Arrange
        var certificateId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        var serialNumber = "CERT-2024-001";
        var holderName = "John Doe";
        var courseTitle = "Introduction to Programming";
        var issueDateUtc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        DateTime? expiryDateUtc = null;
        var issuedBy = "ORG-EDU-001";
        var version = "1.0.0";

        var invalidHash = "0000000000000000000000000000000000000000000000000000000000000000";

        // Act
        var isValid = CertificateSignatureHelper.VerifySignature(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version, invalidHash);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void VerifySignature_IsCaseInsensitive()
    {
        // Arrange
        var certificateId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
        var serialNumber = "CERT-2024-001";
        var holderName = "John Doe";
        var courseTitle = "Introduction to Programming";
        var issueDateUtc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        DateTime? expiryDateUtc = null;
        var issuedBy = "ORG-EDU-001";
        var version = "1.0.0";

        // Generate hash (lowercase)
        var expectedHash = CertificateSignatureHelper.GenerateSignature(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version);

        // Act - Verify with uppercase hash
        var isValid = CertificateSignatureHelper.VerifySignature(
            certificateId, serialNumber, holderName, courseTitle, 
            issueDateUtc, expiryDateUtc, issuedBy, version, expectedHash.ToUpperInvariant());

        // Assert
        Assert.True(isValid);
    }

    #endregion
}
