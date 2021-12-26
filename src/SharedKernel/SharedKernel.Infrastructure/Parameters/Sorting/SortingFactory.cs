using System;

using VShop.SharedKernel.Infrastructure.Parameters.Sorting.Contracts;

namespace VShop.SharedKernel.Infrastructure.Parameters.Sorting
{
    public class SortingFactory
    {
        public static ISortingParameters Create(string sort)
            => new SortingParameters(string.IsNullOrWhiteSpace(sort) ? 
                Array.Empty<string>() : sort.Split(','));
    }
}