namespace Store.Services.User.Data;

public record User(string id, string? firstName, string? lastName, string email, bool isActive);