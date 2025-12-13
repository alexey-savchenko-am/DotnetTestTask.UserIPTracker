using Microsoft.EntityFrameworkCore;
using UserIpTracker.Domain;

namespace UserIpTracker.Infrastructure.Data;

internal sealed class UserDbContext
    : DbContext
{
    public DbSet<User> Users { get; set; }

    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
    }
}
