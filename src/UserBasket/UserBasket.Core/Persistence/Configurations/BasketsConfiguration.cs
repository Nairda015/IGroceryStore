using IGroceryStore.UserBasket.Core.Entities;
using IGroceryStore.UserBasket.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IGroceryStore.UserBasket.Core.Persistence.Configurations;

internal class BasketsConfiguration : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.OwnerId)
            .IsRequired()
            .HasConversion(x => x.Value, x => new UserId(x));
    }
}
