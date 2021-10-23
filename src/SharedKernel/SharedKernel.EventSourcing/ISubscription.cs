using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.SharedKernel.EventSourcing
{
    public interface ISubscription
    {
        Task ProjectAsync(IDomainEvent @event, EventMetadata eventMetadata);
    }
}