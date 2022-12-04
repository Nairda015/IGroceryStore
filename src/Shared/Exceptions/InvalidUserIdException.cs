using System.Net;

namespace IGroceryStore.Shared.Exceptions;

public class InvalidUserIdException : GroceryStoreException
{

    public InvalidUserIdException() : base("Invalid User Id")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}