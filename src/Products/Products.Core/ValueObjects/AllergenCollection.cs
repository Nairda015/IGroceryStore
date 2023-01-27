using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGroceryStore.Products.Entities;
using IGroceryStore.Shared.Common;

namespace IGroceryStore.Products.ValueObjects
{
    internal sealed class AllergenCollection : IEnumerable<Allergen>, IEquatable<AllergenCollection>
    {
        private HashSet<Allergen> _allergens;
        private int? _hashCode;
        public static AllergenCollection Empty => new(Enumerable.Empty<Allergen>());

        public IReadOnlySet<Allergen> Allergens
        {
            get { return _allergens.AsReadOnly(); }
            private set { _allergens = value.ToHashSet(); }
        }

        public AllergenCollection(IEnumerable<Allergen> allergens)
        {
            _allergens = allergens.ToHashSet();
        }

        public bool Equals(AllergenCollection? other) => 
            other != null && _allergens.SetEquals(other!._allergens);

        public override bool Equals(object? other) => 
            Equals(other as AllergenCollection);
        public IEnumerator<Allergen> GetEnumerator() => 
            _allergens.GetEnumerator();
        public override int GetHashCode()
        {
            if (_hashCode != null) return _hashCode.Value;

            var hashCode = new HashCode();
            foreach(var allergen in _allergens)
            {
                hashCode.Add(allergen.GetHashCode());
            }

            return _hashCode ??= hashCode.ToHashCode();
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public static bool operator ==(AllergenCollection? left, AllergenCollection? right) =>
            left != null && left.Equals(right);
        public static bool operator !=(AllergenCollection? left, AllergenCollection? right) =>
            left == null || !left.Equals(right);
        public void Add(Allergen allergen) => _allergens.Add(allergen);
    }
}
