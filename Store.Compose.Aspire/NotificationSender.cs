using Aspire.Hosting.Azure;
using Projects;

namespace Store.Compose.Aspire;

public static class NotificationSender
{
    public static IResourceBuilder<ProjectResource> AddNotificationSender(this IDistributedApplicationBuilder builder,
        IResourceBuilder<AzureServiceBusResource> serviceBus)
    {
        return builder
            .AddProject<Store_Services_Notification_Sender>("NotificationSenderJob")
            .WithEnvironment("Communication:0:TopicName", "NotificationEvents")
            .WithReference(serviceBus, "ServiceBusConnection")
            .WaitFor(serviceBus);
    }
}