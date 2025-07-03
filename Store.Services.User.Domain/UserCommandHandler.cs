using Store.Infrastructure.Communication.Abstractions;
using Store.Services.Shared.Events.User;
using Store.Services.User.Domain.Commands;

namespace Store.Services.User.Domain;

public sealed class UserCommandHandler(IUserRepository repository, ICommunicationBus bus)
    : ICommandHandler<CreateUserCommand>
{
    public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (!UserValidator.IsCreateCommandValid(request))
        {
            return false;
        }

        var user = await repository.AddAsync(new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
        }, cancellationToken);

        if (user is null)
        {
            return false;
        }

        await bus.PublishAsync(new UserCreated
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt
        }, cancellationToken);

        return true;
    }
}