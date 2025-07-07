using Store.Infrastructure.Communication.Abstractions;

namespace Store.Services.Shared.Messages.User;

public class UserCreated : Message
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public DateTime CreatedAt { get; init; }
}