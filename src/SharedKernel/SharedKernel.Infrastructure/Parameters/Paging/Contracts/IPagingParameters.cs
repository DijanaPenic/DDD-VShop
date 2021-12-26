namespace VShop.SharedKernel.Infrastructure.Parameters.Paging.Contracts
{
    public interface IPagingParameters
    {
        int PageIndex { get; }
        int PageSize { get; }
    }
}