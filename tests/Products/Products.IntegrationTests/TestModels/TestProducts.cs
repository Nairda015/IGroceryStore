using Bogus;
using IGroceryStore.Products.Entities;
using IGroceryStore.Products.ValueObjects;
using IGroceryStore.Shared.ValueObjects;

namespace IGroceryStore.Products.IntegrationTests.TestModels;

internal static class TestProducts
{
    private static readonly Faker<Product> CreateProductGenerator = new ProductFaker();

    public static readonly List<Product> Products = CreateProductGenerator.Generate(10);
    public static readonly Product Product = Products.First();

    private sealed class ProductFaker : Faker<Product>
    {
        public ProductFaker()
        {
            CustomInstantiator(ResolveConstructor);
        }

        private Product ResolveConstructor(Faker faker)
        {
            var units = new[] { Unit.Centimeter, Unit.Gram, Unit.Milliliter, Unit.Piece };

            return new Product
            {
                Id = new ProductId(faker.Random.UInt()),
                Name = new ProductName(faker.Commerce.ProductName()),
                Description = new Description(faker.Commerce.ProductDescription()),
                Quantity = new Quantity(faker.Random.UInt(1, 20) * 100, faker.PickRandom(units)),
                CountryId = new CountryId(TestCountries.Country.Id),
                CategoryId = new CategoryId(TestCategories.Category.Id),
                BrandId = new BrandId(TestBrands.Brand.Id),
                ImageUrl = new Uri(faker.Internet.Url()),
                BarCode = new BarCode(faker.Random.UInt(100_000_000, 900_000_000).ToString())
            };
        }
    }
}
