using Microsoft.EntityFrameworkCore;
using Store.Services.Notification.Domain;

namespace Store.Services.Notification.Data;

public sealed class ReceiverInfoRepository(NotificationDbContext dbContext) : IReceiverInfoRepository
{
    public async Task<Domain.ReceiverInfo?> AddAsync(Domain.ReceiverInfo receiverInfo,
        CancellationToken token = default)
    {
        var entity = NotificationConverter.Convert(receiverInfo);
        entity.CreatedAt = DateTime.UtcNow;
        await dbContext.AddAsync(entity, token);
        await dbContext.SaveChangesAsync(token);
        return NotificationConverter.Convert(entity);
    }

    public async Task<Domain.ReceiverInfo?> GetAsync(int id, CancellationToken token = default)
    {
        var entity = await dbContext.ReceiverInfos.SingleOrDefaultAsync(x => x.Id == id, token);
        return entity is null ? null : NotificationConverter.Convert(entity);
    }

    public async Task<Domain.ReceiverInfo?> GetAsync(string email, CancellationToken token = default)
    {
        var entity = await dbContext.ReceiverInfos.SingleOrDefaultAsync(x => x.Email == email, token);
        return entity is null ? null : NotificationConverter.Convert(entity);
    }

    public async Task<Domain.ReceiverInfo?> GetByUserIdAsync(int userId, CancellationToken token = default)
    {
        var entity = await dbContext.ReceiverInfos.SingleOrDefaultAsync(x => x.UserId == userId, token);
        return entity is null ? null : NotificationConverter.Convert(entity);
    }
}