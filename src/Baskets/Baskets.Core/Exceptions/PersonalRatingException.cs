using System.Net;
using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.Baskets.Exceptions;

public class PersonalRatingException : GroceryStoreException
{
    public PersonalRatingException() : base("Personal rating must be between 0 and 10")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
