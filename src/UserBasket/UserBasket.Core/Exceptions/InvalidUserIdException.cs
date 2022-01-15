using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.UserBasket.Core.Exceptions;

public class InvalidUserIdException : GroceryStoreException
{
    public Guid Id { get; }

    public InvalidUserIdException(Guid id) : base("Invalid User Id")
        => Id = id;

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}