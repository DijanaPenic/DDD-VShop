using MediatR;
using VShop.SharedKernel.Infrastructure;

namespace VShop.SharedKernel.Messaging.Commands.Publishing.Contracts
{
    public interface ICommandHandler<in TCommand, TData> : IRequestHandler<TCommand, Result<TData>>
        where TCommand : ICommand<TData>
    {
	
    }
    
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
        where TCommand : ICommand
    {
	
    }
}