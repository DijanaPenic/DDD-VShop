using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Messaging.Commands.Publishing.Contracts
{
    public interface ICommandBus
    {
        Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> command, CancellationToken cancellationToken = default);
        Task<object> SendAsync(object command, CancellationToken cancellationToken = default);
    }
}