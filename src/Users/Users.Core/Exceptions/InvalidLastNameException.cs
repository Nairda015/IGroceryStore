using System.Net;
using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.Users.Exceptions;

public class InvalidLastNameException : GroceryStoreException
{
    public string LastName { get; }
    public InvalidLastNameException(string lastName) : base($"Invalid last name: {lastName}")
        => LastName = lastName;

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
