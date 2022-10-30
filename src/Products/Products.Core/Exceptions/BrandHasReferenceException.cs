using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.Exceptions
{
    internal class BrandHasReferenceException : GroceryStoreException
    {
        public BrandHasReferenceException(ulong id)
    :   base($"Brand with id {id} has assigned product. Cannot remove assigned brand.")
        {
        }

        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    }
}
