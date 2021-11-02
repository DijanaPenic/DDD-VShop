using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Messaging;

// TODO - is this correct project for this class
namespace VShop.SharedKernel.EventSourcing.Contracts
{
    public interface IIntegrationRepository
    {
        Task SaveAsync(IIntegrationEvent @event);
        Task<IEnumerable<IIntegrationEvent>> LoadAsync();
    }
}