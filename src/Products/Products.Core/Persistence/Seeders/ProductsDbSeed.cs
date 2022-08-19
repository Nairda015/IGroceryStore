using Bogus;
using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.ValueObjects;

namespace IGroceryStore.Products.Core.Persistence.Seeders;

internal static class ProductsDbSeed
{
    private static ProductsDbContext _context;

    public static async Task SeedSampleDataAsync(this ProductsDbContext context)
    {
        _context = context;
        
        Randomizer.Seed = new Random(1);

        if (!_context.Countries.Any()) await SeedCountries();
        if (!_context.Brands.Any()) await SeedBrands();
        if (!_context.Categories.Any()) await SeedCategories();
        await _context.SaveChangesAsync();

        if (!_context.Products.Any()) await SeedProducts();
        await _context.SaveChangesAsync();
    }

    private static async Task SeedProducts()
    {
        var units = new[] {Unit.Centimeter, Unit.Gram, Unit.Milliliter, Unit.Piece};
        var countriesIds = _context.Countries
            .Select(x => x.Id)
            .ToList();

        var categoriesIds = _context.Categories
            .Select(x => x.Id)
            .ToList();

        var brandsIds = _context.Brands
            .Select(x => x.Id)
            .ToList();

        var productsFaker = new Faker<Product>()
            .RuleFor(x => x.Id, x => new ProductId(x.Random.UInt()))
            .RuleFor(x => x.Name, x => new ProductName(x.Commerce.ProductName()))
            .RuleFor(x => x.Description, x => new Description(x.Commerce.ProductDescription()))
            .RuleFor(x => x.Quantity, x => new Quantity(x.Random.UInt(1, 20) * 100, x.PickRandom(units)))
            .RuleFor(x => x.CountryId, x => new CountryId(x.PickRandom(countriesIds)))
            .RuleFor(x => x.CategoryId, x => new CategoryId(x.PickRandom(categoriesIds)))
            .RuleFor(x => x.BrandId, x => new BrandId(x.PickRandom(brandsIds)))
            .RuleFor(x => x.ImageUrl, x => new Uri(x.Internet.Url()))
            .RuleFor(x => x.BarCode, x => new BarCode(x.Random.UInt(100_000_000, 900_000_000).ToString()));

        var product = productsFaker.Generate(100);
        await _context.Products.AddRangeAsync(product);
    }

    private static async Task SeedCountries()
    {
        var countriesFaker = new Faker<Country>()
            .RuleFor(x => x.Id, x => new CountryId((ulong) x.UniqueIndex))
            .RuleFor(x => x.Name, x => x.Address.Country())
            .RuleFor(x => x.Code, x => x.Address.CountryCode());

        var countries = countriesFaker.Generate(50).DistinctBy(x => x.Code);
        await _context.Countries.AddRangeAsync(countries);
    }

    private static async Task SeedBrands()
    {
        var brandsFaker = new Faker<Brand>()
            .RuleFor(x => x.Id, x => new BrandId((ulong) x.UniqueIndex))
            .RuleFor(x => x.Name, x => x.Company.CompanyName());

        var brands = brandsFaker.Generate(20);
        await _context.Brands.AddRangeAsync(brands);
    }

    private static async Task SeedCategories()
    {
        var categoriesFaker = new Faker<Category>()
            .RuleFor(x => x.Id, x => new CategoryId((ulong) x.UniqueIndex))
            .RuleFor(x => x.Name, x => x.Commerce.Categories(1).First());

        var categories = categoriesFaker.Generate(20);
        await _context.Categories.AddRangeAsync(categories);
    }
}