using System.ComponentModel.DataAnnotations;

namespace AI.DependencyInjection.Models;

/// <summary>
/// User model
/// </summary>
public class User
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public string? PhoneNumber { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public UserRole Role { get; set; } = UserRole.User;
    
    // Computed properties
    public string FullName => $"{FirstName} {LastName}";
    public int Age => DateTime.UtcNow.Year - CreatedAt.Year;
}

/// <summary>
/// User roles enumeration
/// </summary>
public enum UserRole
{
    User = 0,
    Admin = 1,
    SuperAdmin = 2
}

/// <summary>
/// Create user request model
/// </summary>
public class CreateUserRequest
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Phone]
    public string? PhoneNumber { get; set; }
    
    public UserRole Role { get; set; } = UserRole.User;
}

/// <summary>
/// Update user request model
/// </summary>
public class UpdateUserRequest
{
    [StringLength(100)]
    public string? FirstName { get; set; }
    
    [StringLength(100)]
    public string? LastName { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }
    
    [Phone]
    public string? PhoneNumber { get; set; }
    
    public bool? IsActive { get; set; }
    
    public UserRole? Role { get; set; }
}

/// <summary>
/// Email message model
/// </summary>
public class EmailMessage
{
    [Required]
    [EmailAddress]
    public string To { get; set; } = string.Empty;
    
    [EmailAddress]
    public string? From { get; set; }
    
    public List<string> Cc { get; set; } = new();
    
    public List<string> Bcc { get; set; } = new();
    
    [Required]
    public string Subject { get; set; } = string.Empty;
    
    [Required]
    public string Body { get; set; } = string.Empty;
    
    public bool IsHtml { get; set; } = false;
    
    public List<EmailAttachment> Attachments { get; set; } = new();
    
    public EmailPriority Priority { get; set; } = EmailPriority.Normal;
    
    public DateTime? ScheduledAt { get; set; }
    
    public Dictionary<string, string> Headers { get; set; } = new();
}

/// <summary>
/// Email attachment model
/// </summary>
public class EmailAttachment
{
    [Required]
    public string FileName { get; set; } = string.Empty;
    
    [Required]
    public byte[] Content { get; set; } = Array.Empty<byte>();
    
    public string ContentType { get; set; } = "application/octet-stream";
    
    public string? ContentId { get; set; }
}

/// <summary>
/// Email priority enumeration
/// </summary>
public enum EmailPriority
{
    Low = 0,
    Normal = 1,
    High = 2
}

/// <summary>
/// Bulk email result model
/// </summary>
public class BulkEmailResult
{
    public int TotalEmails { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<EmailFailure> Failures { get; set; } = new();
    public TimeSpan ProcessingTime { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Email failure details
/// </summary>
public class EmailFailure
{
    public string Email { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? ExceptionMessage { get; set; }
}

/// <summary>
/// Email statistics model
/// </summary>
public class EmailStatistics
{
    public int TotalEmailsSent { get; set; }
    public int EmailsSentToday { get; set; }
    public int EmailsSentThisWeek { get; set; }
    public int EmailsSentThisMonth { get; set; }
    public double AverageDeliveryTime { get; set; }
    public double SuccessRate { get; set; }
    public DateTime LastEmailSent { get; set; }
}

/// <summary>
/// Log level enumeration
/// </summary>
public enum LogLevel
{
    Debug = 0,
    Information = 1,
    Warning = 2,
    Error = 3,
    Critical = 4
}

/// <summary>
/// Cache statistics model
/// </summary>
public class CacheStatistics
{
    public long TotalEntries { get; set; }
    public long HitCount { get; set; }
    public long MissCount { get; set; }
    public double HitRatio => TotalEntries > 0 ? (double)HitCount / (HitCount + MissCount) : 0;
    public long MemoryUsageBytes { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Notification type enumeration
/// </summary>
public enum NotificationType
{
    Info = 0,
    Warning = 1,
    Error = 2,
    Success = 3
}

/// <summary>
/// Notification request model
/// </summary>
public class NotificationRequest
{
    public int UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public Dictionary<string, object> Data { get; set; } = new();
}

/// <summary>
/// Bulk notification result model
/// </summary>
public class BulkNotificationResult
{
    public int TotalNotifications { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<NotificationFailure> Failures { get; set; } = new();
    public TimeSpan ProcessingTime { get; set; }
}

/// <summary>
/// Notification failure details
/// </summary>
public class NotificationFailure
{
    public int UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? ExceptionMessage { get; set; }
}

/// <summary>
/// Notification preferences model
/// </summary>
public class NotificationPreferences
{
    public bool EmailNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; } = false;
    public bool PushNotifications { get; set; } = true;
    public List<NotificationType> AllowedTypes { get; set; } = new();
    public Dictionary<string, bool> CategoryPreferences { get; set; } = new();
}

/// <summary>
/// Service configuration model
/// </summary>
public class ServiceConfiguration
{
    public string ServiceName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public Dictionary<string, object> Settings { get; set; } = new();
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxRetryAttempts { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
}