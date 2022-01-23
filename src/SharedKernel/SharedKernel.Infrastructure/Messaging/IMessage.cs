using IProtoData = Google.Protobuf.IMessage;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public interface IMessage : IMessageContext , IProtoData
    {

    }
}