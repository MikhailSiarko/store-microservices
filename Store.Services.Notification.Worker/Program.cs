using Store.Services.Notification.Data;
using Store.Services.Notification.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddData();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();