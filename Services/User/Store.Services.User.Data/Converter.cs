namespace Store.Services.User.Data;

internal static class Converter
{
    public static Domain.Models.User? Convert(User? user)
    {
        if (user is null)
            return null;

        return new Domain.Models.User
        {
            Id = Guid.Parse(user.id),
            Email = user.email,
            FirstName = user.firstName,
            LastName = user.lastName,
            IsActive = user.isActive
        };
    }

    public static User? Convert(Domain.Models.User? user)
    {
        return user is null
            ? null
            : new User(user.Id.ToString(), user.FirstName, user.LastName, user.Email, user.IsActive);
    }
}