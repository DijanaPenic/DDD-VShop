using VShop.SharedKernel.Infrastructure.Parameters.Paging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Parameters.Paging
{
    public class PagingParameters : IPagingParameters
    {
        public int PageIndex { get; }
        public int PageSize { get; }

        public PagingParameters(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}