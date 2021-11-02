using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.EventSourcing.Contracts
{
    public interface ISubscription
    {
        Task ProjectAsync(IMessage message, MessageMetadata metadata);
    }
}