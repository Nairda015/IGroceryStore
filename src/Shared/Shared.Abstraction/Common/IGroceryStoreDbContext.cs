namespace IGroceryStore.Shared.Abstraction.Common;

public interface IGroceryStoreDbContext
{
    Task Seed();
}