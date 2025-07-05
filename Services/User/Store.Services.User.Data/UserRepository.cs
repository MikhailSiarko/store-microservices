using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using Store.Services.User.Domain.Storage;

namespace Store.Services.User.Data;

internal class UserRepository(IConfiguration configuration) : IUserRepository
{
    private CosmosClient? _client;

    public async Task<Domain.Models.User?> GetAsync(Guid id, CancellationToken token = default)
    {
        var container = await GetContainerAsync(token);
        if (container is null)
            return null;

        var user = await container.ReadItemAsync<User>(id.ToString(), new PartitionKey(id.ToString()),
            cancellationToken: token);
        return Converter.Convert(user);
    }

    public async Task<Domain.Models.User?> GetAsync(string email, CancellationToken token = default)
    {
        var container = await GetContainerAsync(token);
        if (container is null)
            return null;

        var queryDefinition = new QueryDefinition("SELECT TOP 1 * FROM c WHERE c.email = @email")
            .WithParameter("@email", email);
        var iterator = container.GetItemQueryIterator<User>(queryDefinition);
        var response = await iterator.ReadNextAsync(token);
        return response.Count != 0 ? null : response.Select(Converter.Convert).SingleOrDefault();
    }

    public async Task<Domain.Models.User?> AddAsync(Domain.Models.User user, CancellationToken token = default)
    {
        var container = await GetContainerAsync(token);
        if (container is null)
            return null;

        var entity = Converter.Convert(user)!;
        entity = await container.CreateItemAsync(entity, new PartitionKey(entity.id), cancellationToken: token);
        return Converter.Convert(entity);
    }

    public async Task UpdateAsync(Domain.Models.User user, CancellationToken token = default)
    {
        var container = await GetContainerAsync(token);
        if (container is null)
            return;

        var entity = Converter.Convert(user)!;
        await container.UpsertItemAsync(entity, new PartitionKey(entity.id), cancellationToken: token);
    }

    public async Task DeleteAsync(Guid id, CancellationToken token = default)
    {
        var container = await GetContainerAsync(token);
        if (container is null)
            return;

        await container.DeleteItemAsync<User>(id.ToString(), new PartitionKey(id.ToString()), cancellationToken: token);
    }

    public async Task<Domain.Models.User[]> GetAllAsync(CancellationToken token = default)
    {
        var container = await GetContainerAsync(token);
        if (container is null)
            return [];

        List<User> users = [];
        var iterator = container.GetItemLinqQueryable<User>().ToFeedIterator();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(token);
            users.AddRange(response);
        }

        return users.Select(Converter.Convert).ToArray()!;
    }

    private async Task InitializeAsync(CancellationToken token = default)
    {
        if (_client is null)
        {
            _client = new CosmosClient(configuration.GetConnectionString("UserServiceDb"), new CosmosClientOptions
            {
                UseSystemTextJsonSerializerWithOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                }
            });
            var result =
                await _client.CreateDatabaseIfNotExistsAsync(configuration["Database"], cancellationToken: token);
            if (result is not null)
            {
                await result.Database.CreateContainerIfNotExistsAsync(configuration["Container"],
                    configuration["PartitionKeyPath"], cancellationToken: token);
            }
        }
    }

    private async Task<Container?> GetContainerAsync(CancellationToken token = default)
    {
        await InitializeAsync(token);
        return _client?
            .GetContainer(configuration["Database"], configuration["Container"]);
    }
}