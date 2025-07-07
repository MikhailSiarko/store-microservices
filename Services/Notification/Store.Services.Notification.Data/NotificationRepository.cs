using Microsoft.EntityFrameworkCore;
using Store.Services.Notification.Domain;

namespace Store.Services.Notification.Data;

public sealed class NotificationRepository(NotificationDbContext dbContext) : INotificationRepository
{
    public async Task<Domain.Notification?> AddAsync(Domain.Notification notification,
        CancellationToken token = default)
    {
        var entity = Converter.Convert(notification);
        entity.CreatedAt = DateTime.UtcNow;
        await dbContext.Notifications.AddAsync(entity, token);
        await dbContext.SaveChangesAsync(token);
        return Converter.Convert(entity);
    }

    public async Task<Domain.Notification?> GetAsync(int id, CancellationToken token = default)
    {
        var entity = await dbContext.Notifications.SingleOrDefaultAsync(x => x.Id == id, token);
        return entity is null ? null : Converter.Convert(entity);
    }

    public async Task<Domain.Notification[]> GetAsync(string email, CancellationToken token = default)
    {
        var entities = await dbContext.Notifications.Where(x => x.Email == email).ToArrayAsync(token);
        return entities.Select(Converter.Convert).ToArray();
    }

    public async Task<Domain.Notification[]> GetByUserIdAsync(Guid userId, CancellationToken token = default)
    {
        var entities = await dbContext.Notifications.Where(x => x.UserId == userId).ToArrayAsync(token);
        return entities.Select(Converter.Convert).ToArray();
    }

    public async Task MarkAsSent(Domain.Notification notification, CancellationToken token = default)
    {
        var entity = await dbContext.Notifications.SingleOrDefaultAsync(x => x.Id == notification.Id, token);
        if (entity is null)
            return;

        entity.SentAt = notification.SentAt;
        entity.IsSent = true;
        await dbContext.SaveChangesAsync(token);
    }
}