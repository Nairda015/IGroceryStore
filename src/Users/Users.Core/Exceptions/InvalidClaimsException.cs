using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Users.Core.Exceptions;

public class InvalidClaimsException : GroceryStoreException
{
    public InvalidClaimsException(string message) : base(message)
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}