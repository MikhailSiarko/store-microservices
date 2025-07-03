namespace Store.Services.User.Domain;

public interface IUserRepository
{
    Task<User?> GetAsync(int id, CancellationToken token = default);
    Task<User?> GetAsync(string email, CancellationToken token = default);
    Task<User?> AddAsync(User user, CancellationToken token = default);
    Task UpdateAsync(User user, CancellationToken token = default);
    Task DeleteAsync(int id, CancellationToken token = default);
    Task<User[]> GetAllAsync(CancellationToken token = default);
}