using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Stores.Core.Exceptions;

public class InvalidRatingException : GroceryStoreException
{
    public double Rating { get; }

    public InvalidRatingException(double rating) : base("Rating must be between 0 and 5")
        => Rating = rating;

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}