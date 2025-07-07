using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Store.Services.Notification.Domain;

namespace Store.Services.Notification.Data;

public sealed class ReceiverInfoRepository(IConfiguration configuration) : RepositoryBase(configuration["ReceiverInfoContainer"]!, configuration), IReceiverInfoRepository
{
    public async Task<Domain.ReceiverInfo?> AddAsync(Domain.ReceiverInfo receiverInfo,
        CancellationToken token = default)
    {
        var container = await GetContainerAsync(token);
        if (container is null)
            return null;

        var entity = Converter.Convert(receiverInfo);
        await container.CreateItemAsync(entity, new PartitionKey(entity.id.ToString()), cancellationToken: token);
        return Converter.Convert(entity);
    }
}