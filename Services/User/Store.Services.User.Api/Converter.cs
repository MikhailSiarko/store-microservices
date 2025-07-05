using Store.Services.User.Api.Models;

namespace Store.Services.User.Api;

public static class Converter
{
    public static UserModel Convert(Domain.Models.User user) => new()
        { Id = user.Id, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName };
    
    public static Domain.Models.User Convert(CreateUserModel user) => new()
        { Email = user.Email, FirstName = user.FirstName, LastName = user.LastName };
}