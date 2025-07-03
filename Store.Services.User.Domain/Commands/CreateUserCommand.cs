using Store.Infrastructure.Communication.Abstractions;

namespace Store.Services.User.Domain.Commands;

public class CreateUserCommand : Command
{
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}