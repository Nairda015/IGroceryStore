using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.ValueObjects;

public class AllergenId
{
    public ulong Value { get; }

    public AllergenId(ulong value)
    {
        //TODO: validate
        //if (value == Guid.Empty) throw new InvalidAllergenIdException();
        Value = value;
    }
    
    public static implicit operator ulong(AllergenId id) => id.Value;
    public static implicit operator AllergenId(ulong id) => new(id);
}

public class InvalidAllergenIdException : GroceryStoreException
{
    public InvalidAllergenIdException() : base("Invalid Allergen Id")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}