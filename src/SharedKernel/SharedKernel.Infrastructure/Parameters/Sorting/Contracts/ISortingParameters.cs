using System.Collections.Generic;

namespace VShop.SharedKernel.Infrastructure.Parameters.Sorting.Contracts
{
    public interface ISortingParameters
    {
        IReadOnlyList<ISortingPair> Sorters { get; }
    }
}