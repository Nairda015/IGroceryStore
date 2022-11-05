using Bogus;
using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.ValueObjects;

namespace Products.IntegrationTests;

internal static class TestCountries
{
    private static readonly Faker<Country> CountryGenerator = new CountryFaker();

    public static readonly List<Country> Countries = CountryGenerator.Generate(10);
    public static readonly Country Country = Countries.First();

    private sealed class CountryFaker : Faker<Country>
    {
        public CountryFaker()
        {
            CustomInstantiator(ResolveConstructor);
        }

        private Country ResolveConstructor(Faker faker)
        {
            return new Country
            {
                Id = new CountryId((ulong)faker.UniqueIndex),
                Name = faker.Address.Country(),
                Code = faker.Address.CountryCode()
            };
        }
    }
}
