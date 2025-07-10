using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Store.Infrastructure.Communication.Abstractions;

namespace Store.Infrastructure.Communication.Implementations;

public sealed class Bus(
    CommunicationOptions options,
    IConfiguration configuration,
    IHostEnvironment environment,
    IServiceScopeFactory serviceScopeFactory)
    : IBus, IAsyncDisposable
{
    private CancellationTokenSource? _cancellationTokenSource;

    private readonly Dictionary<string, List<Type>> _handlers = new();
    private readonly HashSet<Type> _eventTypes = [];

    private readonly ServiceBusClient _serviceBusClient = new(configuration.GetConnectionString("Messaging")!,
        new ServiceBusClientOptions
        {
            Identifier = environment.ApplicationName
        });

    private readonly List<ServiceBusProcessor> _processors = [];

    public async Task PublishAsync(Message message, CancellationToken token = default)
    {
        await using var sender = _serviceBusClient.CreateSender(options.TopicName);
        var messageType = message.GetType();
        var serviceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(message, messageType));
        serviceBusMessage.ApplicationProperties.Add("MessageType", messageType.Name);
        await sender.SendMessageAsync(serviceBusMessage, token);
    }

    public Task SubscribeAsync<TAssemblyMarker>(CancellationToken token = default)
    {
        if (options.Subscriptions == null || options.Subscriptions.Length == 0)
        {
            return Task.CompletedTask;
        }

        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
        ScanForConsumers<TAssemblyMarker>();
        foreach (var subscription in options.Subscriptions)
        {
            var processor = _serviceBusClient.CreateProcessor(options.TopicName, subscription);
            processor.ProcessMessageAsync += ProcessMessageAsync;
            processor.ProcessErrorAsync += ProcessErrorAsync;
            _processors.Add(processor);
        }

        return Task.WhenAll(_processors.Select(x => x.StartProcessingAsync(_cancellationTokenSource.Token)));
    }

    private void ScanForConsumers<TAssemblyMarker>()
    {
        var consumerTypes = typeof(TAssemblyMarker)
            .Assembly
            .GetTypes()
            .Where(type =>
                type.GetInterfaces()
                    .Any(i => i.IsGenericType &&
                              i.GetGenericTypeDefinition() == typeof(IMessageHandler<>)))
            .ToList();

        foreach (var consumerType in consumerTypes)
        {
            var interfaces = consumerType
                .GetInterfaces()
                .Where(i => i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IMessageHandler<>));

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

        try
        {
            if (messageType is null || !_handlers.ContainsKey(messageType))
            {
                return;
            }

            await ProcessEvent(messageType, message);
        }
        catch (Exception)
        {
            // log error
        }
        finally
        {
            await e.CompleteMessageAsync(e.Message);
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
                var concreteType = typeof(IMessageHandler<>).MakeGenericType(messageType!);
                await (Task)concreteType.GetMethod(nameof(IMessageHandler<>.HandleAsync))
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

        foreach (var processor in _processors)
        {
            processor.ProcessMessageAsync -= ProcessMessageAsync;
            processor.ProcessErrorAsync -= ProcessErrorAsync;
            await processor.StopProcessingAsync();
            await processor.DisposeAsync();
        }

        _processors.Clear();
        await _serviceBusClient.DisposeAsync();
    }
}