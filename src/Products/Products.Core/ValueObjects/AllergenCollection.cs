using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGroceryStore.Products.Entities;

namespace IGroceryStore.Products.ValueObjects
{
    internal sealed class AllergenCollection : IEnumerable<Allergen>, IEquatable<AllergenCollection>
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
        public static bool operator ==(AllergenCollection? left, AllergenCollection? right)
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
