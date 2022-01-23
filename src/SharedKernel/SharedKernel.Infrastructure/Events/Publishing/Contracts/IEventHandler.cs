using MediatR;

namespace VShop.SharedKernel.Infrastructure.Events.Publishing.Contracts
{
    public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IBaseEvent
    {
        
    }
}