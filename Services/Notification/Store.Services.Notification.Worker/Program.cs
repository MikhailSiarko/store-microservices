using Store.Services.Notification.Data;
using Store.Services.Notification.Domain;
using Store.Services.Notification.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDomain();
builder.Services.AddData(builder.Configuration);
builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();