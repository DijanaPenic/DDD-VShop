using IProtoData = Google.Protobuf.IMessage;

namespace VShop.SharedKernel.Messaging
{
    public interface IMessage : IMessageContext , IProtoData
    {

    }
}