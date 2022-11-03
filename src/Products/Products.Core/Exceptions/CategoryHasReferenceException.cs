using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Exceptions;

public class CategoryHasReferenceException : GroceryStoreException
{
    public CategoryHasReferenceException(ulong id)
        : base($"Category with id {id} has assigned product. Cannot remove assigned category.")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
