using Projects;

namespace Store.Compose.Aspire;

public static class UserService
{
    public static IResourceBuilder<ProjectResource> AddUserService(this IDistributedApplicationBuilder builder,
        IResourceBuilder<AzureCosmosDBResource> cosmosDb, IResourceBuilder<IResourceWithConnectionString> messaging)
    {
        var cosmosDbConfig = builder.Configuration.GetSection("UserService:CosmosDb");
        var database = cosmosDb
            .AddCosmosDatabase("UserServiceDb", cosmosDbConfig["DatabaseId"]);
        var container = database
            .AddContainer("UsersContainer", cosmosDbConfig["PartitionKeyPath"]!, cosmosDbConfig["ContainerId"]);

        var databaseName = builder.AddParameter("CosmosDbDatabase", cosmosDbConfig["DatabaseId"]!);
        var containerName = builder.AddParameter("CosmosDbContainer", cosmosDbConfig["ContainerId"]!);
        var partitionKeyPath = builder.AddParameter("partitionKeyPath", cosmosDbConfig["PartitionKeyPath"]!);

        return builder
            .AddProject<Store_Services_User_Api>("UserService")
            .WithEnvironment("Database", databaseName)
            .WithEnvironment("Container", containerName)
            .WithEnvironment("PartitionKeyPath", partitionKeyPath)
            .WithReference(database)
            .WithReference(messaging)
            .WaitFor(container)
            .WaitFor(messaging);
    }
}