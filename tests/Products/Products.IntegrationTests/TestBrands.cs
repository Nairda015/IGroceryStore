using Bogus;
using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.ValueObjects;

namespace Products.IntegrationTests;

internal static class TestBrands
{
    private static readonly Faker<Brand> ProductGenerator = new BrandFaker();

    public static readonly List<Brand> Brands = ProductGenerator.Generate(10);
    public static readonly Brand Brand = Brands.First();

    private sealed class BrandFaker : Faker<Brand>
    {
        public BrandFaker()
        {
            CustomInstantiator(ResolveConstructor);
        }

        private Brand ResolveConstructor(Faker faker)
        {
            var brand = new Brand { Id = new BrandId((ulong)faker.UniqueIndex), Name = faker.Company.CompanyName() };
            
            return brand;
        }
    }
}
