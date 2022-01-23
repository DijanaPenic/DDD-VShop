using MediatR;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Events.Contracts
{
    public interface IBaseEvent : IMessage, INotification
    {
        
    }
}