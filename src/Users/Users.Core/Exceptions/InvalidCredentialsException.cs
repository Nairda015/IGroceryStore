using System.Net;
using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.Users.Exceptions;

public class InvalidCredentialsException : GroceryStoreException
{
    public InvalidCredentialsException() : base("Invalid credentials")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
