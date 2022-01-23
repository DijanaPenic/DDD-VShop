using MediatR;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.SharedKernel.Infrastructure.Events
{
    public interface IBaseEvent : IMessage, INotification
    {
    }
}