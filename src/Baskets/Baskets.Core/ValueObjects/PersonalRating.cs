using IGroceryStore.Baskets.Core.Exceptions;

namespace IGroceryStore.Baskets.Core.ValueObjects;

internal record PersonalRating
{
    public byte Value { get; }

    public PersonalRating(byte value)
    {
        if (value > 10) throw new PersonalRatingException();
        Value = value;
    }
}
