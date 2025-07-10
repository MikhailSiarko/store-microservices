using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Store.Services.Shared.Messages.Notifications;

namespace Store.Services.Notification.Sender;

public class NotificationSender(ILogger<NotificationSender> logger, IConfiguration configuration)
{
    private const string QueueName = "NotificationSender";
    private const string ResultTopicName = "NotificationEvents";
    private const string ConnectionName = "ServiceBusConnection";
    private const string MessageTypeKey = "MessageType";

    [Function(nameof(NotificationSender))]
    public async Task Run(
        [ServiceBusTrigger(QueueName, Connection = ConnectionName)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        if (!message.ApplicationProperties.TryGetValue(MessageTypeKey, out var messageTypeName) ||
            messageTypeName.ToString() != nameof(NotificationCreated))
        {
            await messageActions.CompleteMessageAsync(message);
            return;
        }

        await Task.Delay(2000);

        var json = Encoding.UTF8.GetString(message.Body.ToArray());
        var notificationCreated = JsonSerializer.Deserialize<NotificationCreated>(json);
        if (notificationCreated == null)
        {
            logger.LogError("Error deserializing message");
            return;
        }

        await using var client = new ServiceBusClient(configuration.GetConnectionString(ConnectionName)!);
        await using var sender = client.CreateSender(ResultTopicName);

        try
        {
            var newMessage = new ServiceBusMessage(JsonSerializer.Serialize(new NotificationSent
                { Id = notificationCreated.Id, SentAt = DateTime.UtcNow }));

            newMessage.ApplicationProperties.Add(MessageTypeKey, nameof(NotificationSent));
            await sender.SendMessageAsync(newMessage);

            logger.LogInformation("Message sent to Service Bus successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError("Error sending message: {ExMessage}", ex.Message);
        }
        finally
        {
            await messageActions.CompleteMessageAsync(message);
        }
    }
}