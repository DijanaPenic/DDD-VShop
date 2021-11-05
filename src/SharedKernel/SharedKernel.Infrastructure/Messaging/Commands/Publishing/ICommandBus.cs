using MediatR;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Infrastructure.Messaging.Commands.Publishing
{
    public interface ICommandBus
    {
        Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> command);
    }
}