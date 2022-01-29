using System;

using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

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
        
        public MessageContext(IContext context)
        {
            MessageId = SequentialGuid.Create();
            Context = context;
        }
    }
}