namespace IGroceryStore.Products.Core.ValueObjects;

internal sealed record CategoryId(ulong Id)
{
    public static implicit operator ulong(CategoryId category) => category.Id;
    public static implicit operator CategoryId(ulong value) => new(value);
}