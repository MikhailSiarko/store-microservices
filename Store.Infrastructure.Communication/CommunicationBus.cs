using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Store.Infrastructure.Communication.Abstractions;

namespace Store.Infrastructure.Communication;

public sealed class CommunicationBus(
    IServiceScopeFactory serviceScopeFactory,
    IMediator mediator,
    IConfiguration configuration)
    : ICommunicationBus
{
    private readonly IConfiguration _configuration = configuration.GetSection("RabbitMQ");
    private readonly Dictionary<string, List<(Type Type, CancellationToken Token)>> _handlers = new();
    private readonly List<Type> _eventTypes = [];

    public Task SendAsync(Command command, CancellationToken token = default)
    {
        return mediator.Send(command, token);
    }

    public async Task PublishAsync(Event @event, CancellationToken token = default)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = _configuration["HostName"]!,
            UserName = _configuration["UserName"]!,
            Password = _configuration["Password"]!
        };
        await using var connection = await connectionFactory.CreateConnectionAsync(token);
        if (token.IsCancellationRequested || !connection.IsOpen)
        {
            return;
        }

        await using var channel = await connection.CreateChannelAsync(cancellationToken: token);
        if (token.IsCancellationRequested || channel.IsClosed)
        {
            return;
        }

        var eventType = @event.GetType();
        var eventTypeName = eventType.Name;
        await channel.QueueDeclareAsync(eventTypeName, false, false, false, cancellationToken: token);
        if (token.IsCancellationRequested)
            return;

        var message = JsonSerializer.Serialize(@event, eventType);
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(string.Empty, eventTypeName, body: body, cancellationToken: token);
    }

    public Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken token = default)
        where TEvent : Event
        where TEventHandler : IEventHandler<TEvent>
    {
        var eventName = typeof(TEvent).Name;
        var handlerType = typeof(TEventHandler);

        if (!_eventTypes.Contains(typeof(TEvent)))
        {
            _eventTypes.Add(typeof(TEvent));
        }

        if (!_handlers.TryGetValue(eventName, out var value))
        {
            value = [];
            _handlers.Add(eventName, value);
        }

        if (value.Any(s => s.Type == handlerType))
        {
            throw new ArgumentException(
                $"Handler Type {handlerType.Name} already is registered for '{eventName}'", nameof(handlerType));
        }

        value.Add((handlerType, token));
        return StartBasicConsume<TEvent>(token);
    }
    
    private async Task StartBasicConsume<TEvent>(CancellationToken token = default) where TEvent : Event
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = _configuration["HostName"]!,
            UserName = _configuration["UserName"]!,
            Password = _configuration["Password"]!
        };
        await using var connection = await connectionFactory.CreateConnectionAsync(token);
        if (token.IsCancellationRequested || !connection.IsOpen)
        {
            return;
        }

        await using var channel = await connection.CreateChannelAsync(cancellationToken: token);
        if (token.IsCancellationRequested || channel.IsClosed)
        {
            return;
        }

        var eventTypeName = typeof(TEvent).Name;
        await channel.QueueDeclareAsync(eventTypeName, false, false, false, cancellationToken: token);
        if (token.IsCancellationRequested)
            return;

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += Consumer_Received;
        await channel.BasicConsumeAsync(eventTypeName, true, consumer, cancellationToken: token);
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
    {
        var eventName = e.RoutingKey;
        var message = Encoding.UTF8.GetString(e.Body.ToArray());

        try
        {
            await ProcessEvent(eventName, message);
        }
        catch (TaskCanceledException)
        {
            // ignored
        }
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (_handlers.TryGetValue(eventName, out var subscriptions))
        {
            using var scope = serviceScopeFactory.CreateScope();
            foreach (var subscription in subscriptions)
            {
                var handler = scope.ServiceProvider.GetService(subscription.Type);
                if (handler is null)
                    continue;
                var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                var @event = JsonSerializer.Deserialize(message, eventType!);
                var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType!);
                await (Task)concreteType.GetMethod(nameof(IEventHandler<>.HandleAsync))?.Invoke(handler, [@event, subscription.Token])!;
            }
        }
    }
}