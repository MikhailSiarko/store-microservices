using Store.Infrastructure.Communication;
using Store.Infrastructure.Communication.Implementations;
using Store.Services.Notification.Data;
using Store.Services.Notification.Domain;
using Store.Services.Notification.Worker;

var builder = Host.CreateApplicationBuilder(args);

var communicationOptions = builder.Configuration.GetSection("Communication").Get<CommunicationOptions[]>()!;

builder.Services.AddDomain(communicationOptions);
builder.Services.AddData();
builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();