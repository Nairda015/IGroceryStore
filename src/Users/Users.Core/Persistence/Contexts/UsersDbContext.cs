using System.Reflection;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.Shared.Services;
using IGroceryStore.Users.Core.Entities;
using IGroceryStore.Users.Core.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Users.Core.Persistence.Contexts;

internal class UsersDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly DateTimeService _dateTimeService;
    
    public UsersDbContext(DbContextOptions<UsersDbContext> options, 
        ICurrentUserService currentUserService,
        DateTimeService dateTimeService)
        : base(options)
    {
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }

    public DbSet<User> Users => Set<User>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    entry.Entity.Created = _dateTimeService.Now;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = _currentUserService.UserId;
                    entry.Entity.LastModified = _dateTimeService.Now;
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
