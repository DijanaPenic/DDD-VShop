using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.Integration.Stores.Contracts
{
    public interface IIntegrationEventStore
    {
        Task SaveAsync(MessageEnvelope<IIntegrationEvent> eventEnvelope, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MessageEnvelope<IIntegrationEvent>>> LoadAsync(CancellationToken cancellationToken = default);
    }
}