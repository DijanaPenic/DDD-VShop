using MediatR;

namespace VShop.SharedKernel.Infrastructure.Queries.Contracts
{
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
        where TQuery : IQuery<TResponse>  { }
}