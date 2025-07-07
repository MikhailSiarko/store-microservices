namespace Store.Services.Notification.Domain;

public interface INotificationRepository
{
    Task<Notification?> AddAsync(Notification notification, CancellationToken token = default);
    Task<Notification?> GetAsync(int id, CancellationToken token = default);
    Task<Notification[]> GetAsync(string email, CancellationToken token = default);
    Task<Notification[]> GetByUserIdAsync(Guid userId, CancellationToken token = default);
    Task MarkAsSent(Notification notification, CancellationToken token = default);
}