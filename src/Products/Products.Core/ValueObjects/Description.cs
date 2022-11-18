using System.Net;
using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.Products.ValueObjects;

public record Description
{
    public Description(string? value)
    {
        if (value is null) return;
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidDescriptionException();
        
        Value = value;
    }

    public string? Value { get; }

    public static implicit operator Description(string? description) => new(description);
    public static implicit operator string?(Description description) => description.Value;
}

public class InvalidDescriptionException : GroceryStoreException

{
    public InvalidDescriptionException() : base("Description is invalid")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
