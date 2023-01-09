using System.Collections;
using System.Collections.Immutable;
using IGroceryStore.Products.ValueObjects;
using IGroceryStore.Shared.Common;
using IGroceryStore.Shared.ValueObjects;

namespace IGroceryStore.Products.Entities;

internal class Product : AuditableEntity
{
    public required ProductId Id { get; init; }
    public required ProductName Name { get; set; }
    public required Quantity Quantity { get; set; }
    public Description? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public BarCode? BarCode { get; set; }
    public bool IsObsolete { get; private set; }
    
    public CountryId CountryId { get; set; }
    public Country Country { get; set; }
    public BrandId BrandId {get; set;}
    public Brand Brand { get; set; }
    public CategoryId CategoryId { get; set; }
    public Category Category { get; set; }
    public AllergenCollection Allergens { get; set; }

    internal Product()
    {
    }
    
    public void MarkAsObsolete()
    {
        IsObsolete = true;
    }
    
    public void AddAllergen(Allergen allergen)
    {
        Allergens.Add(allergen);
    }

    public sealed class AllergenCollection : IEnumerable<Allergen>, IEquatable<AllergenCollection>
    {
        private ISet<Allergen> _allergens;
        public static AllergenCollection Empty => new(Enumerable.Empty<Allergen>());

        public IReadOnlySet<Allergen> Allergens
        {
            get { return (IReadOnlySet<Allergen>)_allergens; }
            private set { _allergens = (ISet<Allergen>)value; }
        }

        public AllergenCollection(IEnumerable<Allergen> allergens)
        {
            _allergens = allergens.ToHashSet();
        }

        public bool Equals(AllergenCollection? other)
        {
            if (other == null) return false;

            return _allergens.SetEquals(other!._allergens);
        }

        public override bool Equals(object? other)
        {
            return Equals(other as AllergenCollection);
        }
        public IEnumerator<Allergen> GetEnumerator()
        {
            return _allergens.GetEnumerator();
        }
        public override int GetHashCode()
        {
            return _allergens.Select(x => x != null ? x.GetHashCode() : 0).Aggregate((x, y) => x ^ y);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public static bool operator == (AllergenCollection? left, AllergenCollection? right)
        {
            return left != null && left.Equals(right);
        }
        public static bool operator !=(AllergenCollection? left, AllergenCollection? right)
        {
            return left == null || !left.Equals(right);
        }
        public void Add(Allergen allergen)
        {
            var allergens = Allergens.ToHashSet();
            allergens.Add(allergen);
            Allergens = allergens;
        }
    }
}
