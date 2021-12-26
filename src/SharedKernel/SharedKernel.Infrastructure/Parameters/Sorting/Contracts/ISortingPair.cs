namespace VShop.SharedKernel.Infrastructure.Parameters.Sorting.Contracts
{
    public interface ISortingPair
    {
        bool Ascending { get; }
        string OrderBy { get; }
        string GetSortExpression();
        string GetQuerySortExpression();
    }
}