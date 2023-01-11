using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGroceryStore.Shared.Common
{
    public class ReadOnlySet<T> : IReadOnlySet<T>
    {
        private readonly ISet<T> _set;
        public ReadOnlySet(ISet<T> set) { ArgumentNullException.ThrowIfNull(set); _set = set; }

        public int Count => _set.Count;
        public bool Contains(T item) => _set.Contains(item);
        public bool IsProperSubsetOf(IEnumerable<T> other) => _set.IsProperSubsetOf(other);
        public bool IsProperSupersetOf(IEnumerable<T> other) => _set.IsProperSupersetOf(other);
        public bool IsSubsetOf(IEnumerable<T> other) => _set.IsSubsetOf(other);
        public bool IsSupersetOf(IEnumerable<T> other) => _set.IsSupersetOf(other);
        public bool Overlaps(IEnumerable<T> other) => _set.Overlaps(other);
        public bool SetEquals(IEnumerable<T> other) => _set.SetEquals(other);
        public IEnumerator<T> GetEnumerator() => _set.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    public static class ReadOnlySetExtensions
    {
        public static ReadOnlySet<T> AsReadOnly<T>(this ISet<T> set) => new(set);
    }
}
