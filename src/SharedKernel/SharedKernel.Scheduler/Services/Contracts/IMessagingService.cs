namespace VShop.SharedKernel.Scheduler.Services.Contracts
{
    public interface IMessagingService
    {
        Task SendMessageAsync(Guid commandId, CancellationToken cancellationToken);
    }
}