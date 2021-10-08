using System.Collections.Generic;
using System.Linq;

namespace VShop.SharedKernel.Infrastructure.Helpers
{
    internal static class HashCodeHelper
    {
        public static int CombineHashCodes(IEnumerable<object> objs)
        {
            unchecked
            {
                return objs.Aggregate(17, (current, obj) => current * 23 + (obj?.GetHashCode() ?? 0));
            }
        }
    }
}