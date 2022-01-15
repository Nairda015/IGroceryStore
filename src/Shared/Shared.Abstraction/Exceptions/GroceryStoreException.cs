using System.Net;

namespace IGroceryStore.Shared.Abstraction.Exceptions;

public abstract class GroceryStoreException : Exception
{
    public abstract HttpStatusCode StatusCode { get; } 
    protected GroceryStoreException(string message) : base(message)
    {
        
    }
}