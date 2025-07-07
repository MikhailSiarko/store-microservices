namespace Store.Services.Notification.Data;

public static class Converter
{
    public static Domain.Notification Convert(Notification notification)
    {
        return new Domain.Notification
        {
            Id = notification.id,
            UserId = notification.userId,
            Email = notification.email,
            Title = notification.title,
            Body = notification.body,
            CreatedAt = notification.createdAt,
            SentAt = notification.sentAt,
        };
    }

    public static Notification Convert(Domain.Notification notification)
    {
        return new Notification(
            notification.Id,
            notification.UserId,
            notification.Email,
            notification.Title,
            notification.Body,
            notification.CreatedAt,
            notification.SentAt,
            notification.IsSent
        );
    }
    
    public static Notification Convert(Domain.Notification notification, DateTime createdAt)
    {
        return new Notification(
            notification.Id,
            notification.UserId,
            notification.Email,
            notification.Title,
            notification.Body,
            createdAt,
            notification.SentAt,
            notification.IsSent
        );
    }

    public static ReceiverInfo Convert(Domain.ReceiverInfo receiverInfo)
    {
        return new ReceiverInfo(
            receiverInfo.Id,
            receiverInfo.UserId,
            receiverInfo.Email,
            receiverInfo.CreatedAt
        );
    }

    public static Domain.ReceiverInfo Convert(ReceiverInfo receiverInfo)
    {
        return new Domain.ReceiverInfo
        {
            Id = receiverInfo.id,
            UserId = receiverInfo.userId,
            Email = receiverInfo.email,
            CreatedAt = receiverInfo.createdAt,       
        };
    }
}