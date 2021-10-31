using MediatR;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Application.Commands
{
    public interface ICommandBus
    {
        Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> command);
    }
}