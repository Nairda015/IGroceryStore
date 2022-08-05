namespace IGroceryStore.Products.Core.ValueObjects;

internal record AllergenId(ulong Id)
{
    public static implicit operator ulong(AllergenId allergen) => allergen.Id;
    public static implicit operator AllergenId(ulong value) => new(value);
}