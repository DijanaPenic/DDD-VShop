using MediatR;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public interface IDomainEvent : IMessage, INotification
    {
    
    }
}