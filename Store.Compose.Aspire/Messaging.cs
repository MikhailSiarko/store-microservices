namespace Store.Compose.Aspire;

public static class Messaging
{
    public static IResourceBuilder<RabbitMQServerResource> AddMessaging(this IDistributedApplicationBuilder builder)
    {
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMq");
        var userName = builder.AddParameter("RabbitMqUsername", rabbitMqConfig["Username"]!, secret: true);
        var password = builder.AddParameter("RabbitMqPassword", rabbitMqConfig["Password"]!, secret: true);

        return builder
            .AddRabbitMQ("Messaging", userName, password)
            .WithContainerName("store.messaging")
            .WithManagementPlugin();
    }
}