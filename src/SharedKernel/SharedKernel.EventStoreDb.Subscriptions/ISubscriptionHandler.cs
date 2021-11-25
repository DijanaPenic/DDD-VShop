using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.EventStoreDb.Messaging;

namespace VShop.SharedKernel.EventStoreDb.Subscriptions
{
    public interface ISubscriptionHandler
    {
        Task ProjectAsync(IMessage message, IMessageMetadata metadata, CancellationToken cancellationToken = default);
    }
}