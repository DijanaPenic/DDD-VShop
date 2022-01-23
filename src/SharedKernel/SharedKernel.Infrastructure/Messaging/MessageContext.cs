namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public abstract class MessageContext
    {
        public MessageMetadata Metadata { get; set; }
    }
    
    public interface IMessageContext
    {
        public MessageMetadata Metadata { get; set; }
    }
}