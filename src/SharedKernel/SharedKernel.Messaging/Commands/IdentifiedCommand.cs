using MediatR;

using VShop.SharedKernel.Infrastructure;

namespace VShop.SharedKernel.Messaging.Commands
{
    public class IdentifiedCommand<TCommand, TResponse> : IdentifiedMessage<TCommand>, IIdentifiedCommand<TCommand, TResponse>
        where TCommand : IBaseCommand
    {
        public IdentifiedCommand(TCommand command, MessageMetadata metadata) : base(command, metadata) { }
    }
    
    public interface IIdentifiedCommand<out TCommand, TResponse> : IIdentifiedMessage<TCommand>, IRequest<Result<TResponse>>
        where TCommand : IBaseCommand
    {
        
    }
    
    public class IdentifiedCommand<TCommand> : IdentifiedMessage<TCommand>, IIdentifiedCommand<TCommand>
        where TCommand : IBaseCommand
    {
        public IdentifiedCommand(TCommand command, MessageMetadata metadata) : base(command, metadata) { }
    }
    
    public interface IIdentifiedCommand<out TCommand> : IIdentifiedMessage<TCommand>, IRequest<Result>
        where TCommand : IBaseCommand
    {
        
    }
}