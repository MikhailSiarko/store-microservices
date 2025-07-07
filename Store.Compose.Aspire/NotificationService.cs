using Projects;

namespace Store.Compose.Aspire;

public static class NotificationService
{
    public static IResourceBuilder<ProjectResource> AddNotificationService(this IDistributedApplicationBuilder builder,
        IResourceBuilder<AzureCosmosDBResource> cosmosDb,
        IResourceBuilder<ProjectResource> userService,
        IResourceBuilder<IResourceWithConnectionString> messaging)
    {
        var cosmosDbConfig = builder.Configuration.GetSection("NotificationService:CosmosDb");
        var database = cosmosDb
            .AddCosmosDatabase("NotificationServiceDb", cosmosDbConfig["DatabaseId"]);
        var notificationContainer = database
            .AddContainer("NotificationsContainer", cosmosDbConfig["PartitionKeyPath"]!,
                cosmosDbConfig["NotificationContainerId"]);
        var receiverInfoContainer = database
            .AddContainer("ReceiverInfoContainer", cosmosDbConfig["PartitionKeyPath"]!,
                cosmosDbConfig["ReceiverInfoContainerId"]);

        var notificationContainerName =
            builder.AddParameter("CosmosDbNotificationContainer", cosmosDbConfig["NotificationContainerId"]!);
        var receiverInfoContainerName =
            builder.AddParameter("CosmosDbReceiverInfoContainer", cosmosDbConfig["ReceiverInfoContainerId"]!);

        return builder.AddProject<Store_Services_Notification_Worker>("NotificationService")
            .WithEnvironment("Database", cosmosDbConfig["DatabaseId"]!)
            .WithEnvironment("NotificationContainer", notificationContainerName)
            .WithEnvironment("ReceiverInfoContainer", receiverInfoContainerName)
            .WithEnvironment("PartitionKeyPath", cosmosDbConfig["PartitionKeyPath"]!)
            .WithReference(messaging)
            .WithReference(database)
            .WaitFor(messaging)
            .WaitFor(notificationContainer)
            .WaitFor(receiverInfoContainer)
            .WaitFor(userService);
    }
}