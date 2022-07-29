using System.Text.Json;
using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IGroceryStore.Products.Core.Persistence.Configurations;

internal sealed class ProductsDbConfiguration : IEntityTypeConfiguration<Product>, IEntityTypeConfiguration<Brand>, IEntityTypeConfiguration<Allergen>, IEntityTypeConfiguration<Category>, IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new ProductId(x));
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasConversion(x => x.Value, x => new ProductName(x));

        builder.Property(x => x.Description)
            .HasConversion(x => x.Value, x => new Description(x));
        
        builder.Property(x => x.BarCode)
            .IsRequired(false)
            .HasConversion(x => x.Value, x => new BarCode(x));
        
        builder.Property(x => x.ImageUrl)
            .IsRequired(false)
            .HasConversion(x => x.ToString(), x => new Uri(x));

        builder.Property(x => x.IsObsolete)
            .IsRequired()
            .HasDefaultValue(false);

        // conversion from VO to multiple columns added to the ef 7.0.0 milestone
        builder
            .Property(e => e.Quantity)
            .IsRequired()
            .HasConversion(
                x => JsonSerializer.Serialize(x, new JsonSerializerOptions()),
                x => JsonSerializer.Deserialize<Quantity>(x, new JsonSerializerOptions())!);
        
        builder
            .Property(e => e.Allergens)
            .IsRequired(false)
            .HasConversion(
                x => JsonSerializer.Serialize(x, new JsonSerializerOptions()),
                x => JsonSerializer.Deserialize<List<Allergen>>(x, new JsonSerializerOptions()));
        
        builder.HasOne(x => x.Brand)
            .WithMany()
            .HasForeignKey(x => x.BrandId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(x => x.Country)
            .WithMany()
            .HasForeignKey(x => x.CountryId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(x => x.BrandId)
            .IsRequired()
            .HasConversion(x => x.Value, x => new BrandId(x));
        
        builder.Property(x => x.CategoryId)
            .IsRequired()
            .HasConversion(x => x.Value, x => new CategoryId(x));
        
        builder.Property(x => x.CountryId)
            .IsRequired()
            .HasConversion(x => x.Value, x => new CountryId(x));
    }

    public void Configure(EntityTypeBuilder<Allergen> builder)
    {
        builder.ToTable("Allergens");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new AllergenId(x));
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasConversion(x => x.Value, x => new AllergenName(x));
    }
    
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new BrandId(x));

        builder.Property(x => x.Name)
            .IsRequired();
    }

    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Category");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new CategoryId(x));
        
        builder.Property(x => x.Name)
            .IsRequired();
    }

    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new CountryId(x));
        
        builder.Property(x => x.Name)
            .IsRequired();
    }
}