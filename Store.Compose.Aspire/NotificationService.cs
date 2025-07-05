using Projects;

namespace Store.Compose.Aspire;

public static class NotificationService
{
    public static IResourceBuilder<ProjectResource> AddNotificationService(this IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> userService, IResourceBuilder<RabbitMQServerResource> rabbitMq)
    {
        var sqlServer = builder
            .AddSqlServer("SqlServer")
            .WithContainerName("store.sql.server");

        var sqlDatabase = sqlServer.AddDatabase("NotificationDb", "NotificationServiceDb");

        return builder.AddProject<Store_Services_Notification_Worker>("NotificationService")
            .WithReference(sqlDatabase)
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq)
            .WaitFor(sqlDatabase)
            .WaitFor(userService);
    }
}