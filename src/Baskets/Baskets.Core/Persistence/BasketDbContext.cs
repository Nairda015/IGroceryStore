using System.Reflection;
using IGroceryStore.Baskets.Core.Entities;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Services;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Baskets.Core.Persistence;

public class BasketDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;

    public BasketDbContext(
        DbContextOptions<BasketDbContext> options,
        ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }
    
    internal DbSet<Basket> Baskets => Set<Basket>();
    internal DbSet<User> Users => Set<User>();
    internal DbSet<Product> Products => Set<Product>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = (Guid)_currentUserService.UserId!;
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
        if (Database.IsRelational()) builder.HasDefaultSchema("IGroceryStore.Baskets");

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    public async Task Seed()
    {
        await this.SeedSampleDataAsync();
    }
}