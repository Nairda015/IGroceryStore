using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Users.Core.Exceptions;

public class InvalidUserIdException : GroceryStoreException
{
    public InvalidUserIdException() : base("UserId cannot be null or empty")
    {
    }
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}