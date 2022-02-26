using MediatR;

using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

using IProtoMessage = Google.Protobuf.IMessage;

namespace VShop.SharedKernel.Infrastructure.Events.Contracts
{
    public interface IBaseEvent : IMessage, INotification, IProtoMessage
    {
        
    }
}