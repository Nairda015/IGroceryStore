using System.Net;

namespace IGroceryStore.Shared.Exceptions;

public class InvalidProductIdException : GroceryStoreException
{

    public InvalidProductIdException() : base("Invalid Product Id")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}