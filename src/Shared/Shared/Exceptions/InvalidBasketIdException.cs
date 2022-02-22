using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Shared.Exceptions;

public class InvalidBasketIdException : GroceryStoreException
{

    public InvalidBasketIdException() : base("Invalid Basket Id")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}