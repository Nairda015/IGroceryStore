using IGroceryStore.Shops.Exceptions;

namespace IGroceryStore.Shops.ValueObjects;

public record Rating
{
    public double Value { get; }

    public Rating(double value)
    {
        if (value is < 0 or > 5) throw new InvalidRatingException(value);
        
        Value = value;
    }
    
    public static implicit operator double(Rating rating) => rating.Value;
    public static implicit operator Rating(double value) => new(value);
}

