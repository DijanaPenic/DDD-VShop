using MediatR;

namespace VShop.SharedKernel.Infrastructure.Queries.Contracts;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>, IBaseQuery
{
    
}