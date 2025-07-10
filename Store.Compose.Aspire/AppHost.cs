using Store.Compose.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var messaging = builder.AddMessaging();
var cosmosDb = builder.AddCosmosDb();
builder.AddUserService(cosmosDb, messaging);
builder.AddNotificationService(cosmosDb, messaging);
builder.AddNotificationSender(messaging);

builder.Build().Run();