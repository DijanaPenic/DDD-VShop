using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventSourcing.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventSourcing.Projections
{
    public interface ISubscription
    {
        Task ProjectAsync(IMessage message, IMessageMetadata metadata, CancellationToken cancellationToken = default);
    }
}