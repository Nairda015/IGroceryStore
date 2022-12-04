using System.Net;

namespace IGroceryStore.Shared.Exceptions;

public class InvalidBasketIdException : GroceryStoreException
{

    public InvalidBasketIdException() : base("Invalid Basket Id")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}