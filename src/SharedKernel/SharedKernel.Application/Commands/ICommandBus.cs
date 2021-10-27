using MediatR;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Application.Commands
{
    public interface ICommandBus
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> command);
    }
}