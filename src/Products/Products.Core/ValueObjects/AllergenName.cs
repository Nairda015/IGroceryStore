using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.ValueObjects;

public record AllergenName
{
    public AllergenName(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidAllergenNameException();
        
        Value = value;
    }

    public string Value { get; }

    public static implicit operator AllergenName(string name) => new(name);
    public static implicit operator string(AllergenName name) => name.Value;
}

public class InvalidAllergenNameException : GroceryStoreException
{
    public InvalidAllergenNameException() : base("Allergen name cannot be empty")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}