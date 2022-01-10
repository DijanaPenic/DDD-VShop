namespace VShop.SharedKernel.Messaging
{
    public class IdentifiedMessage<TMessage> : IIdentifiedMessage<TMessage> where TMessage : IMessage
    {
        public TMessage Data { get; }
        public MessageMetadata Metadata { get; }
        
        public IdentifiedMessage(TMessage data, MessageMetadata metadata)
        {
            Data = data;
            Metadata = metadata;
        }
    }
    
    public interface IIdentifiedMessage<out TMessage> where TMessage : IMessage
    {
        TMessage Data { get; }
        MessageMetadata Metadata { get; }
    }
}