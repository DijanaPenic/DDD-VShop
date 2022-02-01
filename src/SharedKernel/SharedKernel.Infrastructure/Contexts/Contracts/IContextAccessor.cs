using System;

using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Contexts.Contracts;

public interface IContextAccessor
{
    public IContext Context { get; set; }
    void ChangeContext(IMessageContext messageContext, Type type);
}