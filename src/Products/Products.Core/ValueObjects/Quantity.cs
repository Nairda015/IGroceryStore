using System.Net;
using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Products.Core.ValueObjects;

internal record Quantity
{
    public float Amount { get; }
    public Unit Unit { get; }
    public Quantity(float amount, Unit unit)
    {
        if (amount < 0) throw new InvalidQuantityException(amount);
        Amount = amount;
        Unit = unit;
    }
};

internal class InvalidQuantityException : GroceryStoreException
{
    public float Quantity { get; }
    public InvalidQuantityException(float quantity) : base($"Quantity can't be less than 0. Amount: {quantity}")
        => Quantity = quantity;

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}