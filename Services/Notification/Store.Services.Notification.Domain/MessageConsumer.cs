using Store.Infrastructure.Communication.Abstractions;
using Store.Services.Shared.Messages.User;

namespace Store.Services.Notification.Domain;

public sealed class MessageConsumer(IReceiverInfoRepository repository)
    : IMessageConsumer<UserCreated>
{
    public async Task HandleAsync(UserCreated message, CancellationToken token = default)
    {
        _ = await repository.AddAsync(new ReceiverInfo
        {
            Email = message.Email,
            UserId = message.Id,
        }, token);
    }
}