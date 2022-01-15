using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.Exceptions;

public class InvalidBarCodeException : GroceryStoreException
{
    public InvalidBarCodeException() : base($"Invalid bar code")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}