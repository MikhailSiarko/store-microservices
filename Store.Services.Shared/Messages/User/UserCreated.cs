using Store.Infrastructure.Communication.Abstractions;

namespace Store.Services.Shared.Messages.User;

public class UserCreated : Message
{
    public Guid Id { get; init; }
    public required string Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public DateTime CreatedAt { get; init; }
}