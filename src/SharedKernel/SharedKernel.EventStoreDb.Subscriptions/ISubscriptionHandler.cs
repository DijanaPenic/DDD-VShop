using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.EventStoreDb.Messaging;

namespace VShop.SharedKernel.EventStoreDb.Subscriptions
{
    public interface ISubscriptionHandler
    {
        Task ProjectAsync
        (
            IMessage message,
            IMessageMetadata metadata,
            IServiceScope scope,
            IDbContextTransaction transaction,
            CancellationToken cancellationToken = default
        );
    }
}