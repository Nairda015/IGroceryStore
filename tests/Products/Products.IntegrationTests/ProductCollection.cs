using Products.IntegrationTests;

namespace IGroceryStore.Products.IntegrationTests;

[CollectionDefinition("ProductCollection")]
public class ProductCollection : ICollectionFixture<ProductApiFactory>
{
}
