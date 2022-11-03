using IGroceryStore.Baskets.Exceptions;

namespace IGroceryStore.Baskets.ValueObjects;

internal record PersonalRating
{
    public byte Value { get; }

    public PersonalRating(byte value)
    {
        if (value > 10) throw new PersonalRatingException();
        Value = value;
    }
}
