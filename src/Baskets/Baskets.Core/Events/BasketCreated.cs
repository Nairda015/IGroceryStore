namespace IGroceryStore.Baskets.Events;

public record BasketCreated(Guid OwnerId, string Name);
