using Store.Compose.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var messaging = builder.AddMessaging();
var cosmosDb = builder.AddCosmosDb();
var userService = builder.AddUserService(cosmosDb, messaging);
builder.AddNotificationService(cosmosDb, userService, messaging);

builder.Build().Run();