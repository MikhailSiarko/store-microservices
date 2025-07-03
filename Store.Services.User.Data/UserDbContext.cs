using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Store.Services.User.Data;

public class UserDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("UserDb"));
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(x => x.Id);
        modelBuilder.Entity<User>().Property(x => x.Id).UseIdentityColumn();
        modelBuilder.Entity<User>().Property(x => x.Email).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<User>().Property(x => x.FirstName).HasMaxLength(25);
        modelBuilder.Entity<User>().Property(x => x.LastName).HasMaxLength(25);
        base.OnModelCreating(modelBuilder);
    }
}