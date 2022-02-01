using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Baskets.Core.Exceptions;

public class InvalidBasketNameException : GroceryStoreException
{
    public string Name { get; }
    
    public InvalidBasketNameException(string name)
        : base($"Invalid basket name: {name}")
        => Name = name;

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}