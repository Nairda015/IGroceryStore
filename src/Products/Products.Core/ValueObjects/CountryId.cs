using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.ValueObjects;

public class CountryId
{
    public ulong Value { get; }

    private CountryId() : this(0)
    {}
    
    public CountryId(ulong value)
    {
        //TODO: validate
        //if (value == Guid.Empty) throw new InvalidCountryIdException();
        Value = value;
    }
    
    public static implicit operator ulong(CountryId id) => id.Value;
    public static implicit operator CountryId(ulong id) => new(id);
}

public class InvalidCountryIdException : GroceryStoreException
{
    public InvalidCountryIdException() : base("Invalid Country Id")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}