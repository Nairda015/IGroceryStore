using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Users.Exceptions;

public class InvalidFirstNameException : GroceryStoreException
{
    public string FirstName { get; }
    public InvalidFirstNameException(string firstName) : base($"Invalid first name: {firstName}")
        => FirstName = firstName;

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
