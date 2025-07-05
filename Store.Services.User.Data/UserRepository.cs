using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using Store.Services.User.Domain;

namespace Store.Services.User.Data;

public class UserRepository(IConfiguration configuration) : IUserRepository
{
    private readonly IConfiguration _configuration = configuration.GetSection("CosmosDb");
    private CosmosClient? _client;

    public async Task<Domain.User?> GetAsync(Guid id, CancellationToken token = default)
    {
        var container = await GetContainerAsync(token);
        if (container is null)
            return null;


        var user = await container.ReadItemAsync<User>(id.ToString(), new PartitionKey(id.ToString()),
            cancellationToken: token);
        return UserConverter.Convert(user);
    }

    public async Task<Domain.User?> AddAsync(Domain.User? user, CancellationToken token = default)
    {
        if (user is null)
            return null;

        var container = await GetContainerAsync(token);
        if (container is null)
            return null;

        var queryDefinition = new QueryDefinition("SELECT TOP 1 * FROM c WHERE c.email = @email")
            .WithParameter("@email", user.Email);
        var iterator = container.GetItemQueryIterator<User>(queryDefinition);
        var response = await iterator.ReadNextAsync(token);
        if (response.Count != 0)
        {
            return null;
        }

        var entity = UserConverter.Convert(user)!;
        entity = await container.UpsertItemAsync(entity, new PartitionKey(entity.id), cancellationToken: token);
        return UserConverter.Convert(entity);
    }

    public async Task UpdateAsync(Domain.User? user, CancellationToken token = default)
    {
        if (user is null)
            return;

        var container = await GetContainerAsync(token);
        if (container is null)
            return;

        var entity = UserConverter.Convert(user)!;
        await container.ReplaceItemAsync(entity, entity.id, new PartitionKey(entity.id), cancellationToken: token);
    }

    public async Task DeleteAsync(Guid id, CancellationToken token = default)
    {
        var container = await GetContainerAsync(token);
        if (container is null)
            return;

        await container.DeleteItemAsync<User>(id.ToString(), new PartitionKey(id.ToString()), cancellationToken: token);
    }

    public async Task<Domain.User[]> GetAllAsync(CancellationToken token = default)
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

        return users.Select(UserConverter.Convert).ToArray()!;
    }

    private async Task InitializeAsync(CancellationToken token = default)
    {
        if (_client is null)
        {
            _client = new CosmosClient(_configuration.GetConnectionString("Default"), new CosmosClientOptions
            {
                UseSystemTextJsonSerializerWithOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                }
            });
            var result =
                await _client.CreateDatabaseIfNotExistsAsync(_configuration["DatabaseId"], cancellationToken: token);
            if (result is not null)
            {
                await result.Database.CreateContainerIfNotExistsAsync(_configuration["ContainerId"],
                    _configuration["PartitionKeyPath"], cancellationToken: token);
            }
        }
    }

    private async Task<Container?> GetContainerAsync(CancellationToken token = default)
    {
        await InitializeAsync(token);
        return _client?
            .GetContainer(_configuration["DatabaseId"], _configuration["ContainerId"]);
    }
}