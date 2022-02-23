using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IGroceryStore.Products.Core.Persistence.Configurations;

internal sealed class ProductsDbConfiguration : IEntityTypeConfiguration<Product>, IEntityTypeConfiguration<Brand>, IEntityTypeConfiguration<Allergen>, IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new ProductId(x));
        
        builder.Property(x => x.Name)
            .HasConversion(x => x.Value, x => new ProductName(x));

        builder.Property(x => x.Description)
            .HasConversion(x => x.Value, x => new Description(x));
        
        builder.Property(x => x.BarCode)
            .HasConversion(x => x.Value, x => new BarCode(x));
        
        builder.Property(x => x.ImageUrl)
            .HasConversion(x => x.ToString(), x => new Uri(x));
        
        // conversion from VO to multiple columns
        // builder.Property(x => x.Quantity)
        //     .HasConversion(x => x., x => new Description(x));
    }

    public void Configure(EntityTypeBuilder<Allergen> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new AllergenId(x));
    }
    
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new BrandId(x));
    }

    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new CategoryId(x));
    }
}