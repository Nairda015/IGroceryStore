using System.Reflection;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.Users.Core.Entities;
using IGroceryStore.Users.Core.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Users.Core.Persistence.Contexts;

public class UsersDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;
    public UsersDbContext(DbContextOptions<UsersDbContext> options, 
        ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<User> Users => Set<User>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = (Guid)(_currentUserService.UserId ?? Guid.Empty);
                    entry.Entity.Created = DateTime.Now;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = _currentUserService.UserId;
                    entry.Entity.LastModified = DateTime.Now;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        if (Database.IsRelational()) builder.HasDefaultSchema("IGroceryStore.Users");

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    public async Task Seed()
    {
        await this.SeedSampleDataAsync();
    }
}