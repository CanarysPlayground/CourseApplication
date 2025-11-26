using CourseRegistration.Domain.Enums;

namespace CourseRegistration.Application.Interfaces;

/// <summary>
/// Service interface for sending notifications
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends a notification to a student about an available spot
    /// </summary>
    Task SendWaitlistNotificationAsync(
        string studentEmail, 
        string studentName, 
        string courseName, 
        int position,
        NotificationType notificationType);

    /// <summary>
    /// Sends an email notification
    /// </summary>
    Task SendEmailAsync(string toEmail, string subject, string body);

    /// <summary>
    /// Sends an in-app notification (placeholder for future implementation)
    /// </summary>
    Task SendInAppNotificationAsync(Guid studentId, string message);
}
