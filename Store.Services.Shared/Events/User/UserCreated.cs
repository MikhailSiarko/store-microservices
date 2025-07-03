using Store.Infrastructure.Communication.Abstractions;

namespace Store.Services.Shared.Events.User;

public class UserCreated : Event
{
    public int Id { get; init; }
    public required string Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public DateTime CreatedAt { get; init; }
}