using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.ValueObjects;

public record AllergenCode
{
    public AllergenCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidAllergenCodeException();
        
        Value = value;
    }

    public string Value { get; }

    public static implicit operator AllergenCode(string code) => new(code);
    public static implicit operator string(AllergenCode code) => code.Value;
}

public class InvalidAllergenCodeException : GroceryStoreException
{
    public InvalidAllergenCodeException() : base("Allergen code cannot be empty")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}