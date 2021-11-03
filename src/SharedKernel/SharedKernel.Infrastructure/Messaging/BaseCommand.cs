namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public abstract record BaseCommand<TResult> : BaseMessage, ICommand<TResult>
    {
        
    }
}