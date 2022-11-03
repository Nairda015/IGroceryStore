using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Users.Exceptions;

internal class IncorrectPasswordException : GroceryStoreException
{
    public IncorrectPasswordException() : base("Incorrect password")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
