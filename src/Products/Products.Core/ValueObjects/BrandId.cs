using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.ValueObjects;

public class BrandId
{
    public ulong Value { get; }

    private BrandId() : this(0)
    {}
    public BrandId(ulong value)
    {
        //TODO: validate
        //if (value == Guid.Empty) throw new InvalidBrandIdException();
        Value = value;
    }
    
    public static implicit operator ulong(BrandId id) => id.Value;
    public static implicit operator BrandId(ulong id) => new(id);
}

public class InvalidBrandIdException : GroceryStoreException
{
    public InvalidBrandIdException() : base("Invalid Brand Id")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}