using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.EventSourcing.Messaging;

namespace VShop.SharedKernel.EventSourcing.Projections
{
    public interface ISubscription
    {
        Task ProjectAsync(IMessage message, IMessageMetadata metadata, CancellationToken cancellationToken = default);
    }
}