namespace IGroceryStore.Products.Core.ValueObjects;

internal sealed record BrandId(ulong Id)
{
    public static implicit operator ulong(BrandId brand) => brand.Id;
    public static implicit operator BrandId(ulong value) => new(value);
}