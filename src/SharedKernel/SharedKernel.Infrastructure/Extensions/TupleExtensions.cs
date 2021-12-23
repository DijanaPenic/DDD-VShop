using System;
using System.Linq;
using System.Collections.Generic;

namespace VShop.SharedKernel.Infrastructure.Extensions
{
    public static class TupleExtensions
    {
        public static (IEnumerable<T>, IEnumerable<T>) Split<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            ILookup<bool, T> lookupItems = source.ToLookup(predicate);
            
            IEnumerable<T> trueList = lookupItems[true];
            IEnumerable<T> falseList = lookupItems[false];

            return (trueList, falseList);
        }
    }
}