using MediatR;

namespace VShop.SharedKernel.Messaging.Events
{
    public interface IBaseEvent : IMessage, INotification
    {
    }
}