using System.Net;
using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.Exceptions
{
    internal class BrandNotFoundException : GroceryStoreException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

        public BrandNotFoundException(BrandId brandId) : base($"Brand with id {brandId} not found")
        {
        }
    }
}
