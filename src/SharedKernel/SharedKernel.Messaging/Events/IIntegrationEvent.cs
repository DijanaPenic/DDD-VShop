using IProtoData = Google.Protobuf.IMessage;

namespace VShop.SharedKernel.Messaging.Events
{
    public interface IIntegrationEvent : IBaseEvent, IProtoData
    {
        
    }
}