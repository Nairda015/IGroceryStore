using IGroceryStore.Baskets.Core.Exceptions;

namespace IGroceryStore.Baskets.Core.ValueObjects;

public class LastLowestPrice
{
    public decimal Value { get; }

    public LastLowestPrice(decimal price)
    {
        if (price < 0) throw new InvalidPriceException();
        Value = price;
    }

    public static implicit operator decimal(LastLowestPrice lastLowestPrice) => lastLowestPrice.Value;
    public static implicit operator LastLowestPrice(decimal price) => new(price);
}
