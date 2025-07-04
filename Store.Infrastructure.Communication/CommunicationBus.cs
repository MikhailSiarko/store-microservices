using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Store.Infrastructure.Communication.Abstractions;

namespace Store.Infrastructure.Communication;

public sealed class CommunicationBus(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
    : ICommunicationBus, IDisposable
{
    private const string ExchangeName = "store-events";

    private readonly IConfiguration _configuration = configuration.GetSection("RabbitMQ");
    private readonly Dictionary<string, List<(Type Type, CancellationToken Token)>> _handlers = new();
    private readonly List<Type> _eventTypes = [];
    private IConnection? _connection;
    private IChannel? _channel;

    public async Task PublishAsync(Message message, CancellationToken token = default)
    {
        await InitializeAsync(token);
        if (_channel is null)
            return;

        var messageType = message.GetType();
        await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Fanout, true, false, cancellationToken: token);
        var messageStr = JsonSerializer.Serialize(message, messageType);
        var body = Encoding.UTF8.GetBytes(messageStr);
        await _channel.BasicPublishAsync(ExchangeName, messageType.Name, body: body, cancellationToken: token,
            mandatory: true, basicProperties: new BasicProperties
            {
                Persistent = true
            });
    }

    public Task SubscribeAsync<TMessage, TMessageConsumer>(CancellationToken token = default)
        where TMessage : Message
        where TMessageConsumer : IMessageConsumer<TMessage>
    {
        var messageName = typeof(TMessage).Name;
        var handlerType = typeof(TMessageConsumer);

        if (!_eventTypes.Contains(typeof(TMessage)))
        {
            _eventTypes.Add(typeof(TMessage));
        }

        if (!_handlers.TryGetValue(messageName, out var value))
        {
            value = [];
            _handlers.Add(messageName, value);
        }

        if (value.Any(s => s.Type == handlerType))
        {
            throw new ArgumentException(
                $"Handler Type {handlerType.Name} already is registered for '{messageName}'", nameof(handlerType));
        }

        value.Add((handlerType, token));
        return StartBasicConsume<TMessage>(token);
    }

    private async Task StartBasicConsume<TMessage>(CancellationToken token = default) where TMessage : Message
    {
        await InitializeAsync(token);
        if (_channel is null)
            return;

        var messageTypeName = typeof(TMessage).Name;
        await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Fanout, true, false, cancellationToken: token);
        var queueDeclareOk = await _channel.QueueDeclareAsync(cancellationToken: token);
        await _channel.QueueBindAsync(queueDeclareOk.QueueName, ExchangeName, messageTypeName,
            cancellationToken: token);
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += Consumer_Received;
        await _channel.BasicConsumeAsync(string.Empty, false, consumer, cancellationToken: token);
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
    {
        var eventName = e.RoutingKey;
        var message = Encoding.UTF8.GetString(e.Body.ToArray());

        try
        {
            await ProcessEvent(eventName, message);
        }
        finally
        {
            var consumer = (AsyncDefaultBasicConsumer)sender;
            await consumer.Channel.BasicAckAsync(e.DeliveryTag, false);
        }
    }

    private async Task ProcessEvent(string messageTypeName, string messageStr)
    {
        if (_handlers.TryGetValue(messageTypeName, out var subscriptions))
        {
            using var scope = serviceScopeFactory.CreateScope();
            foreach (var subscription in subscriptions)
            {
                var handler = scope.ServiceProvider.GetService(subscription.Type);
                if (handler is null)
                    continue;
                var messageType = _eventTypes.SingleOrDefault(t => t.Name == messageTypeName);
                var message = JsonSerializer.Deserialize(messageStr, messageType!);
                var concreteType = typeof(IMessageConsumer<>).MakeGenericType(messageType!);
                await (Task)concreteType.GetMethod(nameof(IMessageConsumer<>.HandleAsync))
                    ?.Invoke(handler, [message, subscription.Token])!;
            }
        }
    }

    private async Task InitializeAsync(CancellationToken token = default)
    {
        if (_channel is null)
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = _configuration["HostName"]!,
                UserName = _configuration["UserName"]!,
                Password = _configuration["Password"]!
            };
            _connection = await connectionFactory.CreateConnectionAsync(token);
            _channel = await _connection.CreateChannelAsync(cancellationToken: token);
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}