using Xunit;
using CourseRegistration.Application.Validators;

namespace CourseRegistration.Tests.Validators;

/// <summary>
/// Unit tests for CertificateValidator covering all certificate validation rules
/// Tests correspond to rules documented in CERTIFICATE_VALIDATION_RULES.md
/// </summary>
public class CertificateValidatorTests
{
    #region Rule 1.3 & 2.3: HolderName Validation Tests

    [Fact]
    public void ValidateHolderName_WithValidName_ReturnsValid()
    {
        // Arrange
        var validName = "John Doe";

        // Act
        var result = CertificateValidator.ValidateHolderName(validName);

        // Assert
        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ValidateHolderName_WithAccentedCharacters_ReturnsValid()
    {
        // Arrange
        var validName = "María García";

        // Act
        var result = CertificateValidator.ValidateHolderName(validName);

        // Assert
        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ValidateHolderName_WithChineseCharacters_ReturnsValid()
    {
        // Arrange
        var validName = "李明";

        // Act
        var result = CertificateValidator.ValidateHolderName(validName);

        // Assert
        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ValidateHolderName_WithSingleCharacter_ReturnsValid()
    {
        // Arrange
        var validName = "A";

        // Act
        var result = CertificateValidator.ValidateHolderName(validName);

        // Assert
        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ValidateHolderName_WithEmptyString_ReturnsInvalid()
    {
        // Arrange
        var invalidName = "";

        // Act
        var result = CertificateValidator.ValidateHolderName(invalidName);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("cannot be null or empty", result.ErrorMessage);
    }

    [Fact]
    public void ValidateHolderName_WithNull_ReturnsInvalid()
    {
        // Act
        var result = CertificateValidator.ValidateHolderName(null);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("cannot be null or empty", result.ErrorMessage);
    }

    [Fact]
    public void ValidateHolderName_WithHtmlTags_ReturnsInvalid()
    {
        // Arrange
        var invalidName = "<script>alert('xss')</script>";

        // Act
        var result = CertificateValidator.ValidateHolderName(invalidName);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("cannot contain HTML tags", result.ErrorMessage);
    }

    [Fact]
    public void ValidateHolderName_ExceedingMaxLength_ReturnsInvalid()
    {
        // Arrange - 121 characters (exceeds max of 120)
        var invalidName = new string('A', 121);

        // Act
        var result = CertificateValidator.ValidateHolderName(invalidName);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("cannot exceed 120 characters", result.ErrorMessage);
    }

    [Fact]
    public void ValidateHolderName_AtMaxLength_ReturnsValid()
    {
        // Arrange - Exactly 120 characters
        var validName = new string('A', 120);

        // Act
        var result = CertificateValidator.ValidateHolderName(validName);

        // Assert
        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ValidateHolderName_WithUnicodeNormalization_HandlesCorrectly()
    {
        // Arrange - é can be represented as single character or e + combining accent
        var name1 = "José"; // é as single character (U+00E9)
        var name2 = "Jose\u0301"; // e + combining acute accent (U+0065 U+0301)

        // Act
        var result1 = CertificateValidator.ValidateHolderName(name1);
        var result2 = CertificateValidator.ValidateHolderName(name2);

        // Assert
        Assert.True(result1.IsValid);
        Assert.True(result2.IsValid);
    }

    #endregion

    #region Rule 2.5: ExpiryDate Validation Tests

    [Fact]
    public void ValidateExpiryDate_WithExpiryAfterIssue_ReturnsValid()
    {
        // Arrange
        var issueDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var expiryDate = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        // Act
        var result = CertificateValidator.ValidateExpiryDate(issueDate, expiryDate);

        // Assert
        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ValidateExpiryDate_WithNullExpiry_ReturnsValid()
    {
        // Arrange
        var issueDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        // Act
        var result = CertificateValidator.ValidateExpiryDate(issueDate, null);

        // Assert
        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ValidateExpiryDate_WithExpiryBeforeIssue_ReturnsInvalid()
    {
        // Arrange
        var issueDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var expiryDate = new DateTime(2024, 1, 14, 10, 0, 0, DateTimeKind.Utc);

        // Act
        var result = CertificateValidator.ValidateExpiryDate(issueDate, expiryDate);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("must be after issue date", result.ErrorMessage);
    }

    [Fact]
    public void ValidateExpiryDate_WithExpiryEqualToIssue_ReturnsInvalid()
    {
        // Arrange
        var issueDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var expiryDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        // Act
        var result = CertificateValidator.ValidateExpiryDate(issueDate, expiryDate);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("must be after issue date", result.ErrorMessage);
    }

    #endregion

    #region Rule 1.9: SignatureHash Validation Tests

    [Fact]
    public void ValidateSignatureHash_WithValidHash_ReturnsValid()
    {
        // Arrange - Valid SHA-256 hash (64 hex characters)
        var validHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

        // Act
        var result = CertificateValidator.ValidateSignatureHash(validHash);

        // Assert
        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ValidateSignatureHash_WithUppercaseHash_ReturnsValid()
    {
        // Arrange
        var validHash = "E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855";

        // Act
        var result = CertificateValidator.ValidateSignatureHash(validHash);

        // Assert
        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ValidateSignatureHash_WithNull_ReturnsInvalid()
    {
        // Act
        var result = CertificateValidator.ValidateSignatureHash(null);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("cannot be null or empty", result.ErrorMessage);
    }

    [Fact]
    public void ValidateSignatureHash_WithInvalidLength_ReturnsInvalid()
    {
        // Arrange - Too short
        var invalidHash = "abc123";

        // Act
        var result = CertificateValidator.ValidateSignatureHash(invalidHash);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("must be exactly 64 hexadecimal characters", result.ErrorMessage);
    }

    [Fact]
    public void ValidateSignatureHash_WithNonHexCharacters_ReturnsInvalid()
    {
        // Arrange - Contains invalid characters (g, h, z)
        var invalidHash = "ghz0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

        // Act
        var result = CertificateValidator.ValidateSignatureHash(invalidHash);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("must contain only hexadecimal characters", result.ErrorMessage);
    }

    #endregion

    #region Rule 1.10: Version Validation Tests

    [Fact]
    public void ValidateVersion_WithValidSemanticVersion_ReturnsValid()
    {
        // Arrange
        var validVersions = new[] { "1.0.0", "2.1.0", "1.0.1", "10.20.30" };

        foreach (var version in validVersions)
        {
            // Act
            var result = CertificateValidator.ValidateVersion(version);

            // Assert
            Assert.True(result.IsValid, $"Version {version} should be valid");
            Assert.Null(result.ErrorMessage);
        }
    }

    [Fact]
    public void ValidateVersion_WithInvalidFormat_ReturnsInvalid()
    {
        // Arrange
        var invalidVersions = new[] { "v1", "1.0", "1", "1.0.0.0", "a.b.c" };

        foreach (var version in invalidVersions)
        {
            // Act
            var result = CertificateValidator.ValidateVersion(version);

            // Assert
            Assert.False(result.IsValid, $"Version {version} should be invalid");
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("semantic versioning", result.ErrorMessage);
        }
    }

    [Fact]
    public void ValidateVersion_WithNull_ReturnsInvalid()
    {
        // Act
        var result = CertificateValidator.ValidateVersion(null);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("cannot be null or empty", result.ErrorMessage);
    }

    #endregion

    #region Rule 1.8: VerificationUrl Validation Tests

    [Fact]
    public void ValidateVerificationUrl_WithValidHttpsUrl_ReturnsValid()
    {
        // Arrange
        var validUrls = new[]
        {
            "https://example.com/verify/550e8400-e29b-41d4-a716-446655440000",
            "https://example.com/verify?serial=CERT-2024-001"
        };

        foreach (var url in validUrls)
        {
            // Act
            var result = CertificateValidator.ValidateVerificationUrl(url);

            // Assert
            Assert.True(result.IsValid, $"URL {url} should be valid");
            Assert.Null(result.ErrorMessage);
        }
    }

    [Fact]
    public void ValidateVerificationUrl_WithHttpUrl_ReturnsInvalid()
    {
        // Arrange
        var httpUrl = "http://example.com/verify";

        // Act
        var result = CertificateValidator.ValidateVerificationUrl(httpUrl);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("must use HTTPS", result.ErrorMessage);
    }

    [Fact]
    public void ValidateVerificationUrl_WithInvalidUrl_ReturnsInvalid()
    {
        // Arrange
        var invalidUrl = "not-a-url";

        // Act
        var result = CertificateValidator.ValidateVerificationUrl(invalidUrl);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("must be a valid URL", result.ErrorMessage);
    }

    [Fact]
    public void ValidateVerificationUrl_WithNull_ReturnsInvalid()
    {
        // Act
        var result = CertificateValidator.ValidateVerificationUrl(null);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("cannot be null or empty", result.ErrorMessage);
    }

    #endregion

    #region Rule 2.1: UTC Timestamp Validation Tests

    [Fact]
    public void ValidateUtcTimestamp_WithUtcDateTime_ReturnsValid()
    {
        // Arrange
        var utcTimestamp = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        // Act
        var result = CertificateValidator.ValidateUtcTimestamp(utcTimestamp);

        // Assert
        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ValidateUtcTimestamp_WithLocalDateTime_ReturnsInvalid()
    {
        // Arrange
        var localTimestamp = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Local);

        // Act
        var result = CertificateValidator.ValidateUtcTimestamp(localTimestamp);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("must be in UTC", result.ErrorMessage);
    }

    [Fact]
    public void ValidateUtcTimestamp_WithUnspecifiedDateTime_ReturnsInvalid()
    {
        // Arrange
        var unspecifiedTimestamp = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Unspecified);

        // Act
        var result = CertificateValidator.ValidateUtcTimestamp(unspecifiedTimestamp);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("must be in UTC", result.ErrorMessage);
    }

    #endregion

    #region Rule 2.4: HTML Sanitization Tests

    [Fact]
    public void SanitizeText_WithPlainText_ReturnsUnchanged()
    {
        // Arrange
        var plainText = "John Doe";

        // Act
        var result = CertificateValidator.SanitizeText(plainText);

        // Assert
        Assert.Equal("John Doe", result);
    }

    [Fact]
    public void SanitizeText_WithHtmlTags_RemovesTags()
    {
        // Arrange
        var htmlText = "<script>alert('xss')</script>";

        // Act
        var result = CertificateValidator.SanitizeText(htmlText);

        // Assert
        Assert.DoesNotContain("<script>", result);
        Assert.DoesNotContain("</script>", result);
    }

    [Fact]
    public void SanitizeText_WithHtmlEntities_DecodesAndRemovesTags()
    {
        // Arrange
        var htmlText = "&lt;script&gt;alert('xss')&lt;/script&gt;";

        // Act
        var result = CertificateValidator.SanitizeText(htmlText);

        // Assert
        Assert.DoesNotContain("<", result);
        Assert.DoesNotContain(">", result);
    }

    [Fact]
    public void SanitizeText_WithNull_ReturnsEmpty()
    {
        // Act
        var result = CertificateValidator.SanitizeText(null);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void SanitizeText_WithUnicodeCharacters_NormalizesCorrectly()
    {
        // Arrange
        var unicodeText = "José García";

        // Act
        var result = CertificateValidator.SanitizeText(unicodeText);

        // Assert
        Assert.Contains("José", result);
        Assert.Contains("García", result);
    }

    #endregion

    #region Rule 1.4: CourseTitle Validation Tests

    [Fact]
    public void ValidateCourseTitle_WithValidTitle_ReturnsValid()
    {
        // Arrange
        var validTitle = "Introduction to Programming";

        // Act
        var result = CertificateValidator.ValidateCourseTitle(validTitle);

        // Assert
        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ValidateCourseTitle_WithNull_ReturnsInvalid()
    {
        // Act
        var result = CertificateValidator.ValidateCourseTitle(null);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("cannot be null or empty", result.ErrorMessage);
    }

    [Fact]
    public void ValidateCourseTitle_WithHtmlTags_ReturnsInvalid()
    {
        // Arrange
        var invalidTitle = "<b>Programming</b>";

        // Act
        var result = CertificateValidator.ValidateCourseTitle(invalidTitle);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("cannot contain HTML tags", result.ErrorMessage);
    }

    #endregion

    #region Rule 1.2: SerialNumber Validation Tests

    [Fact]
    public void ValidateSerialNumber_WithValidFormat_ReturnsValid()
    {
        // Arrange
        var validSerialNumbers = new[] { "CERT-2024-001", "CERT-2024-002" };

        foreach (var serialNumber in validSerialNumbers)
        {
            // Act
            var result = CertificateValidator.ValidateSerialNumber(serialNumber);

            // Assert
            Assert.True(result.IsValid, $"SerialNumber {serialNumber} should be valid");
            Assert.Null(result.ErrorMessage);
        }
    }

    [Fact]
    public void ValidateSerialNumber_WithNull_ReturnsInvalid()
    {
        // Act
        var result = CertificateValidator.ValidateSerialNumber(null);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("cannot be null or empty", result.ErrorMessage);
    }

    [Fact]
    public void ValidateSerialNumber_ExceedingMaxLength_ReturnsInvalid()
    {
        // Arrange - 51 characters (exceeds max of 50)
        var invalidSerialNumber = new string('A', 51);

        // Act
        var result = CertificateValidator.ValidateSerialNumber(invalidSerialNumber);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("cannot exceed 50 characters", result.ErrorMessage);
    }

    [Fact]
    public void ValidateSerialNumber_WithEmail_ReturnsInvalid()
    {
        // Arrange - Contains PII (email)
        var invalidSerialNumber = "john@example.com-CERT-2024";

        // Act
        var result = CertificateValidator.ValidateSerialNumber(invalidSerialNumber);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("cannot contain email", result.ErrorMessage);
    }

    #endregion

    #region Rule 1.7: IssuedBy Validation Tests

    [Fact]
    public void ValidateIssuedBy_WithValidValue_ReturnsValid()
    {
        // Arrange
        var validValues = new[] { "ORG-EDU-001", "DEPT-CS" };

        foreach (var value in validValues)
        {
            // Act
            var result = CertificateValidator.ValidateIssuedBy(value);

            // Assert
            Assert.True(result.IsValid, $"IssuedBy {value} should be valid");
            Assert.Null(result.ErrorMessage);
        }
    }

    [Fact]
    public void ValidateIssuedBy_WithNull_ReturnsInvalid()
    {
        // Act
        var result = CertificateValidator.ValidateIssuedBy(null);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("cannot be null or empty", result.ErrorMessage);
    }

    [Fact]
    public void ValidateIssuedBy_ExceedingMaxLength_ReturnsInvalid()
    {
        // Arrange - 101 characters (exceeds max of 100)
        var invalidValue = new string('A', 101);

        // Act
        var result = CertificateValidator.ValidateIssuedBy(invalidValue);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("cannot exceed 100 characters", result.ErrorMessage);
    }

    #endregion
}
