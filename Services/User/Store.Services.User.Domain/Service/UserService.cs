using Store.Infrastructure.Communication.Abstractions;
using Store.Services.Shared.Messages.User;
using Store.Services.User.Domain.Storage;
using Store.Services.User.Domain.Validators;

namespace Store.Services.User.Domain.Service;

public sealed class UserService(ICommunicationBus bus, IUserRepository repository) : IUserService
{
    public async Task<Models.User[]> GetUsersAsync(CancellationToken token = default)
    {
        var users = await repository.GetAllAsync(token);
        return users.Select(x => new Models.User
        {
            Id = x.Id,
            Email = x.Email,
            FirstName = x.FirstName,
            LastName = x.LastName
        }).ToArray();
    }

    public async Task CreateUserAsync(Models.User? model, CancellationToken token = default)
    {
        if (model is null || !UserValidator.IsEmailValid(model.Email))
        {
            return;
        }

        var user = await repository.GetAsync(model.Email, token);
        if (user is not null)
            return;

        user = new Models.User
        {
            Id = Guid.CreateVersion7(),
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        user = await repository.AddAsync(user, token);
        if (user is null)
            return;

        await bus.PublishAsync(new UserCreated
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        }, token);
    }
}