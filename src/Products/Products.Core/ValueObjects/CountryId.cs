namespace IGroceryStore.Products.Core.ValueObjects;

internal sealed record CountryId(ulong Id)
{
    public static implicit operator ulong(CountryId country) => country.Id;
    public static implicit operator CountryId(ulong value) => new(value);
}