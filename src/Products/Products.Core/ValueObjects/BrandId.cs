using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.ValueObjects;

public class BrandId
{
    public Guid Value { get; }

    public BrandId(Guid value)
    {
        if (value == Guid.Empty) throw new InvalidBrandIdException();
        Value = value;
    }
    
    public static implicit operator Guid(BrandId id) => id.Value;
    public static implicit operator BrandId(Guid id) => new(id);
}

public class InvalidBrandIdException : GroceryStoreException
{
    public InvalidBrandIdException() : base("Invalid Brand Id")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}