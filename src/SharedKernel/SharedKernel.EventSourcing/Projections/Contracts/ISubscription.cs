using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventSourcing.Projections.Contracts
{
    public interface ISubscription
    {
        Task ProjectAsync(IMessage message, IMessageMetadata metadata);
    }
}