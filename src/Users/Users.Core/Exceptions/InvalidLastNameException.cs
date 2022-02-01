using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Users.Core.Exceptions;

public class InvalidLastNameException : GroceryStoreException
{
    public string LastName { get; }
    public InvalidLastNameException(string lastName) : base($"Invalid last name: {lastName}")
        => LastName = lastName;

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}