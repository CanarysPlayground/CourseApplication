using System.Text;
using System.Text.RegularExpressions;

namespace CourseRegistration.Application.Validators;

/// <summary>
/// Validator for certificate data according to the certificate validation rules
/// </summary>
public static class CertificateValidator
{
    private const int HolderNameMinLength = 1;
    private const int HolderNameMaxLength = 120;
    private const int Sha256HashLength = 64;

    /// <summary>
    /// Validates holder name according to Rule 1.3 and 2.3
    /// - Must pass Unicode normalization (NFC)
    /// - Length: 1-120 characters
    /// - Plain text only (no HTML)
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) ValidateHolderName(string? holderName)
    {
        if (string.IsNullOrWhiteSpace(holderName))
        {
            return (false, "Holder name cannot be null or empty");
        }

        // Apply Unicode Normalization Form C (NFC)
        string normalized = holderName.Normalize(NormalizationForm.FormC);

        // Check length after normalization
        if (normalized.Length < HolderNameMinLength)
        {
            return (false, $"Holder name must be at least {HolderNameMinLength} character");
        }

        if (normalized.Length > HolderNameMaxLength)
        {
            return (false, $"Holder name cannot exceed {HolderNameMaxLength} characters");
        }

        // Check for HTML tags
        if (ContainsHtml(normalized))
        {
            return (false, "Holder name cannot contain HTML tags");
        }

        return (true, null);
    }

    /// <summary>
    /// Validates that expiry date is after issue date according to Rule 2.5
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) ValidateExpiryDate(DateTime issueDateUtc, DateTime? expiryDateUtc)
    {
        if (expiryDateUtc.HasValue)
        {
            if (expiryDateUtc.Value <= issueDateUtc)
            {
                return (false, "Expiry date must be after issue date");
            }
        }

        return (true, null);
    }

    /// <summary>
    /// Validates signature hash format according to Rule 1.9
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) ValidateSignatureHash(string? signatureHash)
    {
        if (string.IsNullOrWhiteSpace(signatureHash))
        {
            return (false, "Signature hash cannot be null or empty");
        }

        if (signatureHash.Length != Sha256HashLength)
        {
            return (false, $"Signature hash must be exactly {Sha256HashLength} hexadecimal characters (SHA-256)");
        }

        // Check if it's valid hexadecimal
        if (!IsValidHexString(signatureHash))
        {
            return (false, "Signature hash must contain only hexadecimal characters");
        }

        return (true, null);
    }

    /// <summary>
    /// Validates version format according to Rule 1.10
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) ValidateVersion(string? version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            return (false, "Version cannot be null or empty");
        }

        // Semantic versioning pattern: MAJOR.MINOR.PATCH
        var semverPattern = @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)$";
        if (!Regex.IsMatch(version, semverPattern))
        {
            return (false, "Version must follow semantic versioning format (MAJOR.MINOR.PATCH)");
        }

        return (true, null);
    }

    /// <summary>
    /// Validates verification URL according to Rule 1.8
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) ValidateVerificationUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return (false, "Verification URL cannot be null or empty");
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return (false, "Verification URL must be a valid URL");
        }

        if (uri.Scheme != Uri.UriSchemeHttps)
        {
            return (false, "Verification URL must use HTTPS protocol");
        }

        return (true, null);
    }

    /// <summary>
    /// Validates that timestamps are in UTC according to Rule 2.1
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) ValidateUtcTimestamp(DateTime timestamp)
    {
        if (timestamp.Kind != DateTimeKind.Utc)
        {
            return (false, "Timestamp must be in UTC");
        }

        return (true, null);
    }

    /// <summary>
    /// Sanitizes text input to prevent XSS according to Rule 2.4
    /// </summary>
    public static string SanitizeText(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        // Apply Unicode normalization
        string normalized = input.Normalize(NormalizationForm.FormC);

        // Remove HTML tags
        string sanitized = Regex.Replace(normalized, "<.*?>", string.Empty);

        // Decode HTML entities to prevent double-encoding attacks
        sanitized = System.Net.WebUtility.HtmlDecode(sanitized);

        // Remove any remaining HTML after decoding
        sanitized = Regex.Replace(sanitized, "<.*?>", string.Empty);

        return sanitized.Trim();
    }

    /// <summary>
    /// Checks if text contains HTML tags
    /// </summary>
    private static bool ContainsHtml(string text)
    {
        // Simple check for common HTML patterns
        var htmlPattern = @"<[^>]+>";
        return Regex.IsMatch(text, htmlPattern);
    }

    /// <summary>
    /// Validates if string is valid hexadecimal
    /// </summary>
    private static bool IsValidHexString(string str)
    {
        return Regex.IsMatch(str, @"^[0-9a-fA-F]+$");
    }

    /// <summary>
    /// Validates course title according to Rule 1.4
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) ValidateCourseTitle(string? courseTitle)
    {
        if (string.IsNullOrWhiteSpace(courseTitle))
        {
            return (false, "Course title cannot be null or empty");
        }

        // Check for HTML tags
        if (ContainsHtml(courseTitle))
        {
            return (false, "Course title cannot contain HTML tags");
        }

        return (true, null);
    }

    /// <summary>
    /// Validates serial number format according to Rule 1.2
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) ValidateSerialNumber(string? serialNumber)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
        {
            return (false, "Serial number cannot be null or empty");
        }

        if (serialNumber.Length > 50)
        {
            return (false, "Serial number cannot exceed 50 characters");
        }

        // Serial number should not contain PII (basic check - no @ for email, no spaces for names)
        if (serialNumber.Contains('@'))
        {
            return (false, "Serial number cannot contain email addresses (PII)");
        }

        return (true, null);
    }

    /// <summary>
    /// Validates issued by field according to Rule 1.7
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) ValidateIssuedBy(string? issuedBy)
    {
        if (string.IsNullOrWhiteSpace(issuedBy))
        {
            return (false, "Issued by cannot be null or empty");
        }

        if (issuedBy.Length > 100)
        {
            return (false, "Issued by cannot exceed 100 characters");
        }

        return (true, null);
    }
}
