using System;
using System.Linq;
using System.Collections.Generic;

namespace VShop.SharedKernel.Infrastructure.Extensions
{
    public static class CollectionExtensions
    {
        public static (IEnumerable<T>, IEnumerable<T>) Split<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            ILookup<bool, T> lookupItems = source.ToLookup(predicate);
            
            IEnumerable<T> trueList = lookupItems[true];
            IEnumerable<T> falseList = lookupItems[false];

            return (trueList, falseList);
        }
        
        public static List<T> RemoveRangeAfterLast<T>(this List<T> source, Predicate<T> predicate)
        {
            int lastIndex = source.FindLastIndex(predicate);
            if (lastIndex is -1) return source;
            
            int start = Math.Min(lastIndex + 1, source.Count);
            int remove = Math.Max(0, source.Count - start);

            source.RemoveRange(start, remove);

            return source;
        }
    }
}