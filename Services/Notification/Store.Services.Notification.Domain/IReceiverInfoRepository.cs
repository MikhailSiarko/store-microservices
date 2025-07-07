namespace Store.Services.Notification.Domain;

public interface IReceiverInfoRepository
{
    Task<ReceiverInfo?> AddAsync(ReceiverInfo receiverInfo, CancellationToken token = default);
}