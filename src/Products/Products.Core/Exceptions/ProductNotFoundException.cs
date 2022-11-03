using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Exceptions;

public class ProductNotFoundException : GroceryStoreException
{
    public ulong Id { get; }

    public ProductNotFoundException(ulong id) : base($"Product with id {id} not found")
        => Id = id;
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
}
