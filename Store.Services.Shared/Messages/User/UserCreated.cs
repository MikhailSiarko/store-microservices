using Store.Infrastructure.Communication.Abstractions;

namespace Store.Services.Shared.Messages.User;

public class UserCreated : Message
{
    public int Id { get; init; }
    public required string Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public DateTime CreatedAt { get; init; }
}