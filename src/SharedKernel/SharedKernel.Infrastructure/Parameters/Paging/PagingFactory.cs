using System;

using VShop.SharedKernel.Infrastructure.Parameters.Paging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Parameters.Paging
{
    public class PagingFactory
    {
        public static IPagingParameters Create(int pageIndex, int pageSize)
        {
            if (pageIndex < 1)
                throw new ArgumentException("Parameter must be greater than 1.", nameof(pageIndex));

            if (pageSize < 1)
                throw new ArgumentException("Parameter must be greater than 1.", nameof(pageSize));

            return new PagingParameters(pageIndex, pageSize);
        }
    }
}