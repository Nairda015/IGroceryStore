using System.Net;
using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.Users.Exceptions;

public class UserNotFoundException: GroceryStoreException
{
    public Guid Id { get; }
    public UserNotFoundException(Guid id) : base($"User with id {id} not found")
        => Id = id;
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
}
