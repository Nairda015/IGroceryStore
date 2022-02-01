using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Baskets.Core.Exceptions;

public class InvalidUserIdException : GroceryStoreException
{
    public Guid Id { get; }

    public InvalidUserIdException(Guid id) : base($"Invalid User Id: {id}")
        => Id = id;

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}