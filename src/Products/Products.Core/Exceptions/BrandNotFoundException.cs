using System.Net;
using IGroceryStore.Products.ValueObjects;
using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.Products.Exceptions;

internal class BrandNotFoundException : GroceryStoreException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

    public BrandNotFoundException(BrandId brandId) : base($"Brand with id {brandId} not found")
    {
    }
}
