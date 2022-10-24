using IGroceryStore.Baskets.Core.Subscribers.Products;

namespace IGroceryStore.Baskets.Core.ReadModels;

public record Price(DateOnly Date, decimal Value);
