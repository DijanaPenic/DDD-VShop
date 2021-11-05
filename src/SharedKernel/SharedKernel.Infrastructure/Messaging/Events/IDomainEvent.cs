using MediatR;

namespace VShop.SharedKernel.Infrastructure.Messaging.Events
{
    public interface IDomainEvent : IMessage, INotification
    {
    
    }
}