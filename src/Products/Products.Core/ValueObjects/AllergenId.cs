using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.ValueObjects;

public class AllergenId
{
    public Guid Value { get; }

    public AllergenId(Guid value)
    {
        if (value == Guid.Empty) throw new InvalidAllergenIdException();
        Value = value;
    }
    
    public static implicit operator Guid(AllergenId id) => id.Value;
    public static implicit operator AllergenId(Guid id) => new(id);
}

public class InvalidAllergenIdException : GroceryStoreException
{
    public InvalidAllergenIdException() : base("Invalid Allergen Id")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}