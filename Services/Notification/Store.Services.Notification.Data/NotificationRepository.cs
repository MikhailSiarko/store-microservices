using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Store.Services.Notification.Domain;

namespace Store.Services.Notification.Data;

public sealed class NotificationRepository(IConfiguration configuration)
    : RepositoryBase(configuration["NotificationContainer"]!, configuration), INotificationRepository
{
    public async Task<Domain.Notification?> AddAsync(Domain.Notification notification,
        CancellationToken token = default)
    {
        var container = await GetContainerAsync(token);
        if (container is null)
            return null;

        var entity = Converter.Convert(notification, DateTime.UtcNow);
        await container.CreateItemAsync(entity, new PartitionKey(entity.id.ToString()), cancellationToken: token);
        return Converter.Convert(entity);
    }

    public async Task<Domain.Notification?> GetAsync(Guid id, CancellationToken token = default)
    {
        var container = await GetContainerAsync(token);
        if (container is null)
            return null;

        var itemResponse = await container.ReadItemAsync<Notification>(id.ToString(), new PartitionKey(id.ToString()),
            cancellationToken: token);
        return itemResponse is null ? null : Converter.Convert(itemResponse);
    }

    public async Task<Domain.Notification[]> GetAsync(string email, CancellationToken token = default)
    {
        var container = await GetContainerAsync(token);
        if (container is null)
            return [];

        var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.email = @email")
            .WithParameter("@email", email);
        var iterator = container.GetItemQueryIterator<Notification>(queryDefinition);
        var result = new List<Notification>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(token);
            if (response.Count != 0)
                result.AddRange(response.Resource);
        }

        return result.Select(Converter.Convert).ToArray();
    }

    public async Task<Domain.Notification[]> GetByUserIdAsync(Guid userId, CancellationToken token = default)
    {
        var container = await GetContainerAsync(token);
        if (container is null)
            return [];

        var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.userId = @userId")
            .WithParameter("@userId", userId.ToString());
        var iterator = container.GetItemQueryIterator<Notification>(queryDefinition);
        var result = new List<Notification>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(token);
            if (response.Count != 0)
                result.AddRange(response.Resource);
        }

        return result.Select(Converter.Convert).ToArray();
    }

    public async Task MarkAsSent(Domain.Notification notification, CancellationToken token = default)
    {
        var container = await GetContainerAsync(token);
        if (container is null)
            return;

        var itemResponse = await container.ReadItemAsync<Notification>(notification.Id.ToString(),
            new PartitionKey(notification.Id.ToString()),
            cancellationToken: token);
        if (itemResponse is null)
            return;

        var entity = new Notification(notification.Id, notification.UserId, notification.Email, notification.Title,
            notification.Body, notification.CreatedAt, notification.SentAt, true);

        await container.UpsertItemAsync(entity, new PartitionKey(entity.id.ToString()), cancellationToken: token);
    }
}