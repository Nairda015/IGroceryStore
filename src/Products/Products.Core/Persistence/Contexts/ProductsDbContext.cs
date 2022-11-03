using System.Reflection;
using IGroceryStore.Products.Entities;
using IGroceryStore.Products.Persistence.Seeders;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Persistence.Contexts;

public class ProductsDbContext : DbContext, IGroceryStoreDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly DateTimeService _dateTimeService;
    
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options, 
        ICurrentUserService currentUserService,
        DateTimeService dateTimeService)
        : base(options)
    {
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
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
        if (Database.IsRelational()) builder.HasDefaultSchema("IGroceryStore.Products");

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    public async Task Seed()
    {
        await this.SeedSampleDataAsync();
    }
}
