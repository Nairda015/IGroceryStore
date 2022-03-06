using System.Reflection;
using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.Persistence.Seeders;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Services;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Persistence.Contexts;

internal class ProductsDbContext : DbContext, IGroceryStoreDbContext
{
    private readonly ICurrentUserService _currentUserService;
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options, 
        ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    internal DbSet<Product> Products => Set<Product>();
    internal DbSet<Category> Categories => Set<Category>();
    internal DbSet<Allergen> Allergens => Set<Allergen>();
    internal DbSet<Brand> Brands => Set<Brand>();
    internal DbSet<Country> Countries => Set<Country>();

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
        if (Database.IsRelational()) builder.HasDefaultSchema("IGroceryStore.Products");

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    public async Task Seed()
    {
        await this.SeedSampleDataAsync();
    }
}