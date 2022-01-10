using MediatR;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Messaging.Events;

namespace VShop.SharedKernel.Messaging.Commands.Publishing.Contracts
{
    public interface ICommandHandler<in TCommand, TResponse> 
        : IRequestHandler<IIdentifiedCommand<TCommand, TResponse>, Result<TResponse>>
        where TCommand : IBaseCommand { }
    //
    // public interface ICommandHandler<in TCommand> 
    //     : IRequestHandler<IIdentifiedCommand<TCommand>, Result>
    //     where TCommand : IBaseCommand { }
}