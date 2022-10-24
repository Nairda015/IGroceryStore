using IGroceryStore.Baskets.Core.ValueObjects;
using IGroceryStore.Shared.ValueObjects;

namespace IGroceryStore.Baskets.Core.Events;

public record BasketCreated(Guid OwnerId, string Name);
