using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.Integration.Services.Contracts
{
    public interface IIntegrationEventService
    {
        Task PublishEventsAsync(Guid transactionId, CancellationToken cancellationToken = default);
        Task SaveEventAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default);
    }
}