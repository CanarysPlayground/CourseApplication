using CourseRegistration.Application.Interfaces;
using CourseRegistration.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CourseRegistration.Application.Services;

/// <summary>
/// Service implementation for sending notifications
/// </summary>
public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Sends a notification to a student about an available spot
    /// </summary>
    public async Task SendWaitlistNotificationAsync(
        string studentEmail,
        string studentName,
        string courseName,
        int position,
        NotificationType notificationType)
    {
        var subject = $"Course Waitlist Update - {courseName}";
        var body = $"Dear {studentName},\n\n" +
                   $"A spot has become available in the course '{courseName}'.\n" +
                   $"You are currently in position {position} on the waitlist.\n\n" +
                   $"Please log in to your account to register for the course.\n\n" +
                   $"Best regards,\n" +
                   $"Course Registration System";

        switch (notificationType)
        {
            case NotificationType.Email:
                await SendEmailAsync(studentEmail, subject, body);
                break;
            case NotificationType.InApp:
                // For now, just log - in-app notifications would require additional infrastructure
                _logger.LogInformation($"In-app notification would be sent to {studentEmail}: {subject}");
                break;
            case NotificationType.Both:
                await SendEmailAsync(studentEmail, subject, body);
                _logger.LogInformation($"In-app notification would be sent to {studentEmail}: {subject}");
                break;
        }
    }

    /// <summary>
    /// Sends an email notification (simulated for now)
    /// </summary>
    public Task SendEmailAsync(string toEmail, string subject, string body)
    {
        // In a real implementation, this would integrate with an email service like SendGrid, AWS SES, etc.
        // For now, we'll just log the email
        _logger.LogInformation($"EMAIL NOTIFICATION\n" +
                               $"To: {toEmail}\n" +
                               $"Subject: {subject}\n" +
                               $"Body: {body}");

        return Task.CompletedTask;
    }

    /// <summary>
    /// Sends an in-app notification (placeholder for future implementation)
    /// </summary>
    public Task SendInAppNotificationAsync(Guid studentId, string message)
    {
        // Placeholder for future implementation
        // This would typically store a notification in a database or send via SignalR/WebSocket
        _logger.LogInformation($"In-app notification for student {studentId}: {message}");
        return Task.CompletedTask;
    }
}
