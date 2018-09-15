using System;
using System.Collections.Generic;
using System.Linq;

namespace Feedz.Util.Extensions
{
    public static class EnumerableExtensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items, IEqualityComparer<T> comparer)
            => new HashSet<T>(items, comparer);

        public static IReadOnlyList<IReadOnlyList<T>> Partition<T>(this IEnumerable<T> items, int size = 500)
            => items.Select((id, index) => (id: id, index: index))
                .GroupBy(x => x.index / size, y => y.id)
                .Select(g => g.ToArray())
                .ToArray();

        public static bool None<T>(this IEnumerable<T> items)
            => !items.Any();

        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> items)
            => (items as IReadOnlyList<T>) ?? items.ToArray();
    }

    public static class EnumExtensions
    {
        public static T ParseEnum<T>(this string value)
            => (T) Enum.Parse(typeof(T), value);
    }
}