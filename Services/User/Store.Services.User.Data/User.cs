// ReSharper disable InconsistentNaming
namespace Store.Services.User.Data;

internal record User(string id, string? firstName, string? lastName, string email, bool isActive);