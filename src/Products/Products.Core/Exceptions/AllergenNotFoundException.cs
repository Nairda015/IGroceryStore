using System.Net;
using IGroceryStore.Products.ValueObjects;
using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.Products.Exceptions;

internal class AllergenNotFoundException : GroceryStoreException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

    public AllergenNotFoundException(AllergenId message) : base($"Allergen with id {message} not found")
    {
    }
}
