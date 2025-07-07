using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace Store.Services.Notification.Data;

public abstract class RepositoryBase(string containerName, IConfiguration configuration)
{
    private CosmosClient? _client;
    
    private async Task InitializeAsync(CancellationToken token = default)
    {
        if (_client is null)
        {
            _client = new CosmosClient(configuration.GetConnectionString("NotificationServiceDb"));
            var result =
                await _client.CreateDatabaseIfNotExistsAsync(configuration["Database"], cancellationToken: token);
            if (result is not null)
            {
                await Task.WhenAll(result.Database.CreateContainerIfNotExistsAsync(
                        configuration["NotificationContainer"],
                        configuration["PartitionKeyPath"], cancellationToken: token),
                    result.Database.CreateContainerIfNotExistsAsync(
                        configuration["ReceiverInfoContainer"],
                        configuration["PartitionKeyPath"], cancellationToken: token));
            }
        }
    }

    protected async Task<Container?> GetContainerAsync(CancellationToken token = default)
    {
        await InitializeAsync(token);
        return _client?
            .GetContainer(configuration["Database"], containerName);
    }
}