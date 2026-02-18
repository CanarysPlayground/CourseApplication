namespace CourseRegistration.Domain.Enums;

/// <summary>
/// Certificate status enumeration
/// </summary>
public enum CertificateStatus
{
    /// <summary>
    /// Certificate is active and valid
    /// </summary>
    Active = 0,
    
    /// <summary>
    /// Certificate has been revoked
    /// </summary>
    Revoked = 1,
    
    /// <summary>
    /// Certificate has expired
    /// </summary>
    Expired = 2
}
