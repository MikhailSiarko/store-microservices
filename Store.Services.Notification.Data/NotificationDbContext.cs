using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Store.Services.Notification.Data;

public sealed class NotificationDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<ReceiverInfo> ReceiverInfos { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("NotificationDb"));
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>().HasKey(x => x.Id);
        modelBuilder.Entity<Notification>().Property(x => x.Id).UseIdentityColumn();
        modelBuilder.Entity<Notification>().Property(x => x.Email).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<Notification>().HasIndex(x => x.UserId).IsUnique();
        modelBuilder.Entity<Notification>().Property(x => x.Title).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<Notification>().Property(x => x.Body).IsRequired();
        
        modelBuilder.Entity<ReceiverInfo>().HasKey(x => x.Id);
        modelBuilder.Entity<ReceiverInfo>().Property(x => x.Id).UseIdentityColumn();
        modelBuilder.Entity<ReceiverInfo>().Property(x => x.Email).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<ReceiverInfo>().HasIndex(x => x.UserId);
        modelBuilder.Entity<ReceiverInfo>().HasIndex(x => x.Email).IsUnique();
        base.OnModelCreating(modelBuilder);
    }
}