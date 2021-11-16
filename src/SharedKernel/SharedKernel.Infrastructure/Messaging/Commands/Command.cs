namespace VShop.SharedKernel.Infrastructure.Messaging.Commands
{
    public abstract record Command<TResult> : Message, ICommand<TResult>
    {
        
    }

    public abstract record Command : Message, ICommand
    {
        
    }
}