namespace Store.Services.User.Application.Models;

public sealed class UserModel
{
    public Guid Id { get; set; }
    public string? FirstName  { get; set; }
    public string? LastName  { get; set; }
    public required string Email  { get; set; }
    public bool IsActive  { get; set; }
}