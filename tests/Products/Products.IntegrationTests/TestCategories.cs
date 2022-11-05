using Bogus;
using IGroceryStore.Products.Entities;
using IGroceryStore.Products.ValueObjects;

namespace Products.IntegrationTests;

internal static class TestCategories
{
    private static readonly Faker<Category> CategoryGenerator = new CategoryFaker();
    
    public static readonly List<Category> Categories = CategoryGenerator.Generate(10);
    public static readonly Category Category = Categories.First();

    private sealed class CategoryFaker : Faker<Category>
    {
        public CategoryFaker()
        {
            CustomInstantiator(ResolveConstructor);
        }

        private Category ResolveConstructor(Faker faker)
        {
            return new Category()
            {
                Id = new CategoryId((ulong)faker.UniqueIndex), Name = faker.Commerce.Categories(1).First()
            };
        }
    }
}
