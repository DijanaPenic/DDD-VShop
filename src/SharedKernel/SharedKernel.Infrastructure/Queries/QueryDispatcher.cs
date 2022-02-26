using MediatR;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Queries.Contracts;

namespace VShop.SharedKernel.Infrastructure.Queries;

internal class QueryDispatcher : IQueryDispatcher
{
    private readonly IMediator _mediator;

    public QueryDispatcher(IMediator mediator) => _mediator = mediator;

    public Task<Result<TResponse>> QueryAsync<TResponse>
    (
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default
    ) => _mediator.Send(query, cancellationToken);
    
    public Task<object> QueryAsync(object query, CancellationToken cancellationToken = default)
        => _mediator.Send(query, cancellationToken);
}