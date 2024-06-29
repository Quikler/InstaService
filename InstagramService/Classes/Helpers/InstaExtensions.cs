using System;
using System.Collections.Generic;

namespace InstagramService.Classes.Helpers
{
    internal static class InstaExtensions
    {
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector is null)
                throw new ArgumentNullException(nameof(keySelector));

            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return default;

                var maxItem = enumerator.Current;
                var maxValue = keySelector(maxItem);

                while (enumerator.MoveNext())
                {
                    var currentItem = enumerator.Current;
                    var currentValue = keySelector(currentItem);

                    if (Comparer<TKey>.Default.Compare(currentValue, maxValue) > 0)
                    {
                        maxItem = currentItem;
                        maxValue = currentValue;
                    }
                }

                return maxItem;
            }
        }
    }
}