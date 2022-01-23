using MediatR;

namespace VShop.SharedKernel.Infrastructure.Events.Contracts
{
    public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IBaseEvent
    {
        
    }
}