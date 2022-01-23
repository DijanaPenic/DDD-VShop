using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.Integration.Stores.Contracts
{
    public interface IIntegrationEventStore
    {
        Task SaveAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<IIntegrationEvent>> LoadAsync(CancellationToken cancellationToken = default);
    }
}