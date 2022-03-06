using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Shared.Services;

namespace IGroceryStore.Products.Core.Factories;

internal interface ICountryFactory
{
    Country Create(string name, string code);
}

internal class CountryFactory : ICountryFactory
{
    private readonly ISnowflakeService _snowflakeService;

    public CountryFactory(ISnowflakeService snowflakeService)
    {
        _snowflakeService = snowflakeService;
    }

    public Country Create(string name, string code)
    {
        //TODO: custom exceptions
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code cannot be null or whitespace.", nameof(name));

        return new Country(_snowflakeService.GenerateId(), name, code);
    }
        
}

