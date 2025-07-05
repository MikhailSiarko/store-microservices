namespace Store.Services.User.Domain;

public interface IUserRepository
{
    Task<User?> GetAsync(Guid id, CancellationToken token = default);
    Task<User?> AddAsync(User? user, CancellationToken token = default);
    Task UpdateAsync(User? user, CancellationToken token = default);
    Task DeleteAsync(Guid id, CancellationToken token = default);
    Task<User[]> GetAllAsync(CancellationToken token = default);
}