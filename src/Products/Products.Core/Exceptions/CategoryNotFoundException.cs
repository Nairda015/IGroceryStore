using System.Net;
using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.Products.Exceptions;

public class CategoryNotFoundException : GroceryStoreException
{
    public ulong Id { get; }

    public CategoryNotFoundException(ulong id) : base($"Category with id {id} not found")
        => Id = id;
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
}
