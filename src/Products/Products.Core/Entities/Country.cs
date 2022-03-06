using IGroceryStore.Products.Core.ValueObjects;

namespace IGroceryStore.Products.Core.Entities;

public class Country
{
    public Country()
    {
    }

    internal Country(CountryId id, string name, string code)
    {
        Id = id;
        Name = name;
        Code = code;
    }
    public CountryId Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
}