using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Shared.Exceptions;

public class InvalidProductNameException : GroceryStoreException
{
    public InvalidProductNameException() : base("Invalid product name")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}