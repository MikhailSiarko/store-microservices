using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Store.Infrastructure.Communication.Abstractions;

namespace Store.Infrastructure.Communication;

public sealed class CommunicationBus(
    IConfiguration configuration,
    IHostEnvironment environment,
    IServiceScopeFactory serviceScopeFactory)
    : ICommunicationBus, IAsyncDisposable
{
    private const string TopicName = "StoreEvents";

    private CancellationTokenSource? _cancellationTokenSource;

    private readonly Dictionary<string, List<Type>> _handlers = new();
    private readonly HashSet<Type> _eventTypes = [];

    private readonly ServiceBusClient _serviceBusClient = new(configuration.GetConnectionString("Messaging")!,
        new ServiceBusClientOptions
        {
            Identifier = environment.ApplicationName
        });

    private ServiceBusProcessor? _processor;

    public async Task PublishAsync(Message message, CancellationToken token = default)
    {
        await using var sender = _serviceBusClient.CreateSender(TopicName);
        var messageType = message.GetType();
        var serviceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(message, messageType));
        serviceBusMessage.ApplicationProperties.Add("MessageType", messageType.Name);
        await sender.SendMessageAsync(serviceBusMessage, token);
    }

    public Task SubscribeAsync<TAssemblyMarker>(string subscriptionName, CancellationToken token = default)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
        ScanForConsumers<TAssemblyMarker>();
        _processor = _serviceBusClient.CreateProcessor(TopicName, subscriptionName);
        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;
        return _processor.StartProcessingAsync(_cancellationTokenSource.Token);
    }

    private void ScanForConsumers<TAssemblyMarker>()
    {
        var consumerTypes = typeof(TAssemblyMarker)
            .Assembly
            .GetTypes()
            .Where(type =>
                type.GetInterfaces()
                    .Any(i => i.IsGenericType &&
                              i.GetGenericTypeDefinition() == typeof(IMessageConsumer<>)))
            .ToList();

        foreach (var consumerType in consumerTypes)
        {
            var interfaces = consumerType
                .GetInterfaces()
                .Where(i => i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IMessageConsumer<>));

            foreach (var inter in interfaces)
            {
                var messageType = inter.GetGenericArguments()[0];
                _eventTypes.Add(messageType);
                var messageTypeName = messageType.Name;

                if (!_handlers.TryGetValue(messageTypeName, out var handlers))
                {
                    handlers = [];
                    _handlers.Add(messageTypeName, handlers);
                }

                handlers.Add(inter);
            }
        }
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs e)
    {
        var message = Encoding.UTF8.GetString(e.Message.Body.ToArray());
        var messageType = e.Message.ApplicationProperties["MessageType"].ToString();
        if (messageType is null || !_handlers.ContainsKey(messageType))
        {
            await e.AbandonMessageAsync(e.Message);
            return;
        }

        try
        {
            await ProcessEvent(messageType, message);
            await e.CompleteMessageAsync(e.Message);
        }
        catch (Exception)
        {
            await e.AbandonMessageAsync(e.Message);
        }
    }

    private static Task ProcessErrorAsync(ProcessErrorEventArgs e)
    {
        return Task.CompletedTask;
    }

    private async Task ProcessEvent(string messageTypeName, string messageStr)
    {
        if (_handlers.TryGetValue(messageTypeName, out var subscriptions))
        {
            using var scope = serviceScopeFactory.CreateScope();
            foreach (var subscription in subscriptions)
            {
                var handler = scope.ServiceProvider.GetService(subscription);
                if (handler is null)
                    continue;
                var messageType = _eventTypes.SingleOrDefault(t => t.Name == messageTypeName);
                var message = JsonSerializer.Deserialize(messageStr, messageType!);
                var concreteType = typeof(IMessageConsumer<>).MakeGenericType(messageType!);
                await (Task)concreteType.GetMethod(nameof(IMessageConsumer<>.HandleAsync))
                    ?.Invoke(handler, [message, _cancellationTokenSource?.Token ?? CancellationToken.None])!;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
        }

        if (_processor is not null)
        {
            _processor.ProcessMessageAsync -= ProcessMessageAsync;
            _processor.ProcessErrorAsync -= ProcessErrorAsync;
            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();
        }

        await _serviceBusClient.DisposeAsync();
    }
}