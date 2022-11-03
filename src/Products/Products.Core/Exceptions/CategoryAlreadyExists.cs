using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Exceptions;

public class CategoryAlreadyExists : GroceryStoreException
{
    public CategoryAlreadyExists(string name) : base($"Category with name: {name} already exists.")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
