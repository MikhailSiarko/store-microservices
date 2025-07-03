using Store.Services.User.Application.Models;

namespace Store.Services.User.Application;

public interface IUserService
{
    Task<UserModel[]> GetUsersAsync(CancellationToken token = default);
    Task CreateUserAsync(CreateUserModel model, CancellationToken token = default);
}