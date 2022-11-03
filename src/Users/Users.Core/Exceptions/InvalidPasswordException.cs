using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Users.Exceptions;

public class InvalidPasswordException : GroceryStoreException
{
    public InvalidPasswordException() : base("Invalid password")
    {
    }
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
