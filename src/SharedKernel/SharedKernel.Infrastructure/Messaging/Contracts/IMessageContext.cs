using System;

using VShop.SharedKernel.Infrastructure.Contexts.Contracts;

namespace VShop.SharedKernel.Infrastructure.Messaging.Contracts;

public interface IMessageContext
{
    public Guid MessageId { get; }
    public IContext Context { get; }
}