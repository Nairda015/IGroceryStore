namespace IGroceryStore.Baskets.Core.Events;

public record BasketCreated(Guid OwnerId, string Name);
