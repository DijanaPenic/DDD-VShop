using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Infrastructure.Queries.Contracts
{
    public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
    {
        Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken);
    }
}