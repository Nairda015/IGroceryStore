using IGroceryStore.Shared.Tests;

namespace IGroceryStore.Baskets.IntegrationTests.Configuration;

[CollectionDefinition(TestConstants.Collections.Baskets)]
public class BasketsCollection : ICollectionFixture<BasketApiFactory>
{
}
