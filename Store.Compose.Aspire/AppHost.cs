using Store.Compose.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var rabbitMq = builder.AddMessaging();
var userService = builder.AddUserService(rabbitMq);
builder.AddNotificationService(userService, rabbitMq);

builder.Build().Run();