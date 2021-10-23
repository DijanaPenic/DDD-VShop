using System.Threading.Tasks;

using VShop.SharedKernel.Domain;

namespace VShop.SharedKernel.EventSourcing
{
    public interface ISubscription
    {
        Task ProjectAsync(IDomainEvent @event, EventMetadata eventMetadata);
    }
}