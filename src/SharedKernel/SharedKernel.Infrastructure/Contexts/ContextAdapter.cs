using System;

using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Contexts;

public class ContextAdapter : IContextAdapter
{
    private readonly IContextAccessor _contextAccessor;

    public ContextAdapter(IContextAccessor contextAccessor)
        => _contextAccessor = contextAccessor;

    public void ChangeContext(IMessageContext messageContext, Type type)
    {
        if (messageContext is null) return;
        
        if(_contextAccessor.Context is null) _contextAccessor.Context = messageContext.Context;
        else _contextAccessor.Context.RequestId = messageContext.MessageId;
        
        Console.WriteLine($"{type.Name}: {_contextAccessor.Context.RequestId}");
    }
}