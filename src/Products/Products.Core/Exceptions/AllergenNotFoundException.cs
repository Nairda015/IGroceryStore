using System.Net;
using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.Exceptions;

internal class AllergenNotFoundException : GroceryStoreException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

    public AllergenNotFoundException(AllergenId message) : base($"Allergen with id {message} not found")
    {
    }
}