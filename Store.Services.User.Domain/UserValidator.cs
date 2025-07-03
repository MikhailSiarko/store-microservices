using Store.Services.User.Domain.Commands;

namespace Store.Services.User.Domain;

internal static class UserValidator
{
    public static bool IsCreateCommandValid(CreateUserCommand command)
    {
        return !string.IsNullOrEmpty(command.Email);
    }
}