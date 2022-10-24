using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Baskets.Core.Exceptions;

public class InvalidPriceException : GroceryStoreException
{
    public InvalidPriceException() : base("Price cannot be negative")
        => StatusCode = HttpStatusCode.BadRequest;

    public override HttpStatusCode StatusCode { get; }
}
