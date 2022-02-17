using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Infrastructure.Queries.Contracts;

public interface IQueryDispatcher
{
    Task<Result<TResponse>> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
    Task<object> QueryAsync(object query, CancellationToken cancellationToken = default);
}