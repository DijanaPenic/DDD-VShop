using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Parameters.Sorting.Contracts;

namespace VShop.SharedKernel.Infrastructure.Parameters.Sorting
{
    public class SortingPair : ISortingPair
    {
        public bool Ascending { get; }
        public string OrderBy { get; }

        public SortingPair(string orderBy, bool ascending)
        {
            OrderBy = orderBy;
            Ascending = ascending;
        }

        public virtual string GetSortExpression()
            => $"{OrderBy}{SortingParameters.Separator}{(Ascending ? SortingParameters.AscendingDirection : SortingParameters.DescendingDirection)}";

        public virtual string GetQuerySortExpression()
            => $"{OrderBy.ToSnakeCase()} {(Ascending ? SortingParameters.AscendingDirection : SortingParameters.DescendingDirection)}"; 
    }
}