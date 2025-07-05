namespace Store.Services.User.Api.Models;

public sealed class CreateUserModel
{
    public required string Email { get; set; }
    public string? FirstName  { get; set; }
    public string? LastName  { get; set; }
}