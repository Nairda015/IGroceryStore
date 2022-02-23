using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.ValueObjects;

internal record Unit
{
    public static Unit Gram => new("g");
    public static Unit Milliliter => new("ml");
    public static Unit Centimeter => new("cm");
    public static Unit Piece => new("pc");
    
    public string Name { get; }
    public Unit(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidUnitNameException();
        Name = name;
    }
};

internal class InvalidUnitNameException : GroceryStoreException
{
    public InvalidUnitNameException() : base("Invalid unit name")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}