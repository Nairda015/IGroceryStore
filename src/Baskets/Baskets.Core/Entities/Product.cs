using IGroceryStore.Baskets.Core.ValueObjects;
using IGroceryStore.Shared.ValueObjects;

namespace IGroceryStore.Baskets.Core.Entities;

internal class Product //from the product module
{
    public required ProductId Id { get; init; }
    public required ProductName Name { get; init; }
    public required string Category { get; init; }
    public Dictionary<ulong, decimal> LastPrices { get; set; } = new(); //shopId, price

    public PersonalRating? Rating { get; set; } //move to shop?
}
