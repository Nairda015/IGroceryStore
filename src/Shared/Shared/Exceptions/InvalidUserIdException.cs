using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Shared.Exceptions;

public class InvalidUserIdException : GroceryStoreException
{

    public InvalidUserIdException() : base("Invalid User Id")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}