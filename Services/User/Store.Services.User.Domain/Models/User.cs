namespace Store.Services.User.Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required string Email { get; set; }
    public bool IsActive { get; set; }
}