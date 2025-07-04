namespace Store.Services.Notification.Domain;

public interface IReceiverInfoRepository
{
    Task<ReceiverInfo?> AddAsync(ReceiverInfo receiverInfo, CancellationToken token = default);
    Task<ReceiverInfo?> GetAsync(int id, CancellationToken token = default);
    Task<ReceiverInfo?> GetAsync(string email, CancellationToken token = default);
    Task<ReceiverInfo?> GetByUserIdAsync(int userId, CancellationToken token = default);
}