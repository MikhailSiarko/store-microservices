// ReSharper disable InconsistentNaming
namespace Store.Services.Notification.Data;

public record ReceiverInfo(Guid id, Guid userId, string email, DateTime createdAt);