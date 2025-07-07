using Aspire.Hosting.Azure;

namespace Store.Compose.Aspire;

public static class Messaging
{
    public static IResourceBuilder<AzureServiceBusResource> AddMessaging(this IDistributedApplicationBuilder builder)
    {
        var serviceBus = builder
            .AddAzureServiceBus("Messaging")
            .RunAsEmulator(x =>
            {
                x.WithContainerName("store.messaging")
                    .WithOtlpExporter();
            });

        var topic =
            serviceBus.AddServiceBusTopic("StoreEventsTopic", "StoreEvents");

        topic.AddServiceBusSubscription("Notifications", "Notifications");
        return serviceBus;
    }
}