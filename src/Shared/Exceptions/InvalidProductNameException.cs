using System.Net;

namespace IGroceryStore.Shared.Exceptions;

public class InvalidProductNameException : GroceryStoreException
{
    public InvalidProductNameException() : base("Invalid product name")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}