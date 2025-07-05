namespace Store.Services.User.Domain.Validators;

public static class UserValidator
{
    public static bool IsEmailValid(string email)
    {
        return !string.IsNullOrEmpty(email) && email.Contains('@');
    }
}