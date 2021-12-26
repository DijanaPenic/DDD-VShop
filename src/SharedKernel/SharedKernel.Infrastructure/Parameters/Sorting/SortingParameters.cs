using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Parameters.Sorting.Contracts;

namespace VShop.SharedKernel.Infrastructure.Parameters.Sorting
{
    public class SortingParameters : ISortingParameters
    {
        private readonly List<ISortingPair> _sorters = new();

        public const string AscendingDirection = "asc";
        public const string DescendingDirection = "desc";
        public const char Separator = '|';
        public IReadOnlyList<ISortingPair> Sorters => _sorters;

        public SortingParameters(IReadOnlyCollection<string> sort) => InitializeSorting(sort);

        private void InitializeSorting(IReadOnlyCollection<string> sort)
        {
            if (sort.Count is 0) return;

            foreach (string sortingExpression in sort)
            {
                IList<string> sortParams = sortingExpression.Split(Separator).ToList();

                if (string.IsNullOrWhiteSpace(sortParams[0])) continue;
                
                SortingPair sortingPair = new
                (
                    orderBy: sortParams[0],
                    ascending: sortParams.Count is 1 || (sortParams[1].ToLowerInvariant().StartsWith(AscendingDirection))
                );

                _sorters.Add(sortingPair);
            }
        }
    }
}