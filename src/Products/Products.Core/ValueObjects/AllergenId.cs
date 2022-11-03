namespace IGroceryStore.Products.ValueObjects;

internal record AllergenId(ulong Id)
{
    public static implicit operator ulong(AllergenId allergen) => allergen.Id;
    public static implicit operator AllergenId(ulong value) => new(value);
}
