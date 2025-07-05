namespace Store.Services.User.Domain.Service;

public interface IUserService
{
    Task<Models.User[]> GetUsersAsync(CancellationToken token = default);
    Task CreateUserAsync(Models.User user, CancellationToken token = default);
}