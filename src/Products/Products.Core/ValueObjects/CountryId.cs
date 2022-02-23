using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.ValueObjects;

public class CountryId
{
    public Guid Value { get; }

    public CountryId(Guid value)
    {
        if (value == Guid.Empty) throw new InvalidCountryIdException();
        Value = value;
    }
    
    public static implicit operator Guid(CountryId id) => id.Value;
    public static implicit operator CountryId(Guid id) => new(id);
}

public class InvalidCountryIdException : GroceryStoreException
{
    public InvalidCountryIdException() : base("Invalid Country Id")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}