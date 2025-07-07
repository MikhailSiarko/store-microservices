// ReSharper disable InconsistentNaming

namespace Store.Services.Notification.Data;

public record Notification(
    Guid id,
    Guid userId,
    string email,
    string title,
    string body,
    DateTime createdAt,
    DateTime? sentAt,
    bool isSent);