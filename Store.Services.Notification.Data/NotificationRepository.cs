using Microsoft.EntityFrameworkCore;
using Store.Services.Notification.Domain;

namespace Store.Services.Notification.Data;

public sealed class NotificationRepository(NotificationDbContext dbContext) : INotificationRepository
{
    public async Task<Domain.Notification?> AddAsync(Domain.Notification notification, CancellationToken token = default)
    {
        var entity = NotificationConverter.Convert(notification);
        entity.CreatedAt = DateTime.UtcNow;
        await dbContext.AddAsync(entity, token);
        await dbContext.SaveChangesAsync(token);
        return NotificationConverter.Convert(entity);
    }

    public async Task<Domain.Notification?> GetAsync(int id, CancellationToken token = default)
    {
        var entity = await dbContext.Notifications.SingleOrDefaultAsync(x => x.Id == id, token);
        return entity is null ? null : NotificationConverter.Convert(entity);
    }

    public async Task<Domain.Notification[]> GetAsync(string email, CancellationToken token = default)
    {
        var entities = await dbContext.Notifications.Where(x => x.Email == email).ToArrayAsync(token);
        return entities.Select(NotificationConverter.Convert).ToArray();
    }

    public async Task<Domain.Notification[]> GetByUserIdAsync(int userId, CancellationToken token = default)
    {
        var entities = await dbContext.Notifications.Where(x => x.UserId == userId).ToArrayAsync(token);
        return entities.Select(NotificationConverter.Convert).ToArray();
    }
}