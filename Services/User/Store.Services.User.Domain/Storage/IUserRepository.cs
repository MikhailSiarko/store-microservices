namespace Store.Services.User.Domain.Storage;

public interface IUserRepository
{
    Task<Models.User?> GetAsync(Guid id, CancellationToken token = default);
    Task<Models.User?> GetAsync(string email, CancellationToken token = default);
    Task<Models.User?> AddAsync(Models.User user, CancellationToken token = default);
    Task UpdateAsync(Models.User user, CancellationToken token = default);
    Task DeleteAsync(Guid id, CancellationToken token = default);
    Task<Models.User[]> GetAllAsync(CancellationToken token = default);
}