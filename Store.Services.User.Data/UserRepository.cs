using Microsoft.EntityFrameworkCore;
using Store.Services.User.Domain;

namespace Store.Services.User.Data;

public class UserRepository(UserDbContext context) : IUserRepository
{
    public async Task<Domain.User?> GetAsync(int id, CancellationToken token = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id, token);
        return user is null ? null : UserConverter.Convert(user);
    }

    public async Task<Domain.User?> GetAsync(string email, CancellationToken token = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email, token);
        return user is null ? null : UserConverter.Convert(user);
    }

    public async Task<Domain.User?> AddAsync(Domain.User user, CancellationToken token = default)
    {
        var entity = UserConverter.Convert(user);
        entity.CreatedAt = DateTime.UtcNow;
        await context.Users.AddAsync(entity, token);
        await context.SaveChangesAsync(token);
        return UserConverter.Convert(entity);
    }

    public async Task UpdateAsync(Domain.User user, CancellationToken token = default)
    {
        var entity = UserConverter.Convert(user);
        context.Users.Update(entity);
        await context.SaveChangesAsync(token);
    }

    public async Task DeleteAsync(int id, CancellationToken token = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id, token);
        if (user is not null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync(token);
        }
    }

    public async Task<Domain.User[]> GetAllAsync(CancellationToken token = default)
    {
        var users = await context.Users.ToListAsync(token);
        return users.Select(UserConverter.Convert).ToArray();
    }
}