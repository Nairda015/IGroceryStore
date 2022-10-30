using System.Net;
using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.Exceptions
{
    internal class BrandNotFoundException : GroceryStoreException
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

        public BrandNotFoundException(BrandId message) : base($"Brand with id {message} not found")
        {
        }
    }
}
