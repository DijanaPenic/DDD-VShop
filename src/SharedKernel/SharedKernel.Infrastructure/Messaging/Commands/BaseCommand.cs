namespace VShop.SharedKernel.Infrastructure.Messaging.Commands
{
    public abstract record BaseCommand<TResult> : BaseMessage, ICommand<TResult>
    {
        
    }
}