using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.Exceptions;

public class ProductNotFoundException : GroceryStoreException
{
    public Guid Id { get; }

    public ProductNotFoundException(Guid id) : base($"Product with id {id} not found")
        => Id = id;
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
}