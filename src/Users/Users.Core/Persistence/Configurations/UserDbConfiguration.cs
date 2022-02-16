using IGroceryStore.Users.Core.Entities;
using IGroceryStore.Users.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IGroceryStore.Users.Core.Persistence.Configurations;

internal sealed class UserDbConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        var passwordConverter = new ValueConverter<PasswordHash, string>(l => l.Value,
            l => new PasswordHash(l));
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new UserId(x));
        
        builder.Property(x => x.FirstName)
            .HasConversion(x => x.Value, x => new FirstName(x));
        builder.Property(x => x.Email)
            .HasConversion(x => x.Value, x => new Email(x));
        builder.Property(x => x.LastName)
            .HasConversion(x => x.Value, x => new LastName(x));
        
        builder
            .Property(typeof(List<RefreshToken>), "_refreshTokens")
            .HasColumnName("RefreshTokens")
            .IsRequired(false)
            .HasColumnType("jsonb");
        
        builder
            .Property(typeof(PasswordHash), "_passwordHash")
            .HasConversion(passwordConverter)
            .HasColumnName("PasswordHash");
        
        
        builder
            .Property(typeof(ushort), "_accessFailedCount")
            .HasColumnName("AccessFailedCount");
        
        builder
            .Property(typeof(DateTime), "_lockoutEnd")
            .HasColumnName("LockoutEnd");
        
    }
}