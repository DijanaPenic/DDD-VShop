using System;

using VShop.SharedKernel.Infrastructure.Contexts.Contracts;

namespace VShop.SharedKernel.Infrastructure.Messaging
{
    public class MessageContext : IMessageContext
    {
        public Guid MessageId { get; }
        public IContext Context { get; }

        public MessageContext(Guid messageId, IContext context)
        {
            MessageId = messageId;
            Context = context;
        }
    }
    
    public interface IMessageContext
    {
        public Guid MessageId { get; }
        public IContext Context { get; }
    }
}