using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Users.Core.Exceptions;

public class InvalidTokenException : GroceryStoreException
{
    public InvalidTokenException() : base("Invalid token")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}