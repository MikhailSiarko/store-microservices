namespace Store.Services.Notification.Data;

public static class Converter
{
    public static Domain.Notification Convert(Notification notification)
    {
        return new Domain.Notification
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Email = notification.Email,
            Title = notification.Title,
            Body = notification.Body,
            CreatedAt = notification.CreatedAt,
            SentAt = notification.SentAt,
            IsSent = notification.IsSent,
        };
    }

    public static Notification Convert(Domain.Notification notification)
    {
        return new Notification
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Email = notification.Email,
            Title = notification.Title,
            Body = notification.Body,
            CreatedAt = notification.CreatedAt,
            SentAt = notification.SentAt,
            IsSent = notification.IsSent,
        };
    }

    public static ReceiverInfo Convert(Domain.ReceiverInfo receiverInfo)
    {
        return new ReceiverInfo
        {
            Id = receiverInfo.Id,
            UserId = receiverInfo.UserId,
            Email = receiverInfo.Email,
            CreatedAt = receiverInfo.CreatedAt,
        };
    }

    public static Domain.ReceiverInfo Convert(ReceiverInfo receiverInfo)
    {
        return new Domain.ReceiverInfo
        {
            Id = receiverInfo.Id,
            UserId = receiverInfo.UserId,
            Email = receiverInfo.Email,
            CreatedAt = receiverInfo.CreatedAt,       
        };
    }
}