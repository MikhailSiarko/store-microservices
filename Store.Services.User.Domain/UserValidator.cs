namespace Store.Services.User.Domain;

public static class UserValidator
{
    public static bool IsCreateCommandValid(User user)
    {
        return !string.IsNullOrEmpty(user.Email);
    }
}