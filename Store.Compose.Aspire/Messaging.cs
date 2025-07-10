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

        var userEventsTopic =
            serviceBus.AddServiceBusTopic("UserEvents", "UserEvents");
        var notificationEventsTopic =
            serviceBus.AddServiceBusTopic("NotificationEvents", "NotificationEvents");
        serviceBus.AddServiceBusQueue("NotificationSender", "NotificationSender");

        userEventsTopic.AddServiceBusSubscription("Sub-Notifications", "NotificationService");
        notificationEventsTopic.AddServiceBusSubscription("Sub-NotificationService", "NotificationService");
        return serviceBus;
    }
}