using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Users.Exceptions;

public class InvalidEmailException : GroceryStoreException
{
    public string Email { get; }
    public InvalidEmailException(string email) : base($"Invalid email: {email}")
        => Email = email;

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
