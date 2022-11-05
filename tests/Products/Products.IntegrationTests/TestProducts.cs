using Bogus;
using IGroceryStore.Products.Features.Products.Commands;
using IGroceryStore.Products.ReadModels;
using IGroceryStore.Products.ValueObjects;

namespace Products.IntegrationTests;

internal static class TestProducts
{
    private static readonly Faker<CreateProduct> CreateProductGenerator = new CreateProductFaker();

    private static readonly List<CreateProduct> Products = CreateProductGenerator.Generate(10);
    public static readonly CreateProduct CreateProduct = Products.First();

    private sealed class CreateProductFaker : Faker<CreateProduct>
    {
        public CreateProductFaker()
        {
            CustomInstantiator(ResolveConstructor);
        }

        private CreateProduct ResolveConstructor(Faker faker)
        {
            var units = new[] {Unit.Centimeter, Unit.Gram, Unit.Milliliter, Unit.Piece};

            var body = new CreateProduct.CreateProductBody(
                faker.Commerce.ProductName(),
                new QuantityReadModel(faker.Random.UInt(1, 20) * 100, faker.PickRandom(units)), 
                    new BrandId(TestBrands.Brand.Id),
                    new CountryId(TestCountries.Country.Id),
                    new CategoryId(TestCategories.Category.Id)
            );
            
            return new CreateProduct(body);
        }
    }
}
