using System;
using System.Threading;

using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Contexts;

public sealed class ContextAccessor : IContextAccessor
{
    private static readonly AsyncLocal<ContextHolder> Holder = new();

    public IContext Context
    {
        get => Holder.Value?.Context;
        set
        {
            ContextHolder holder = Holder.Value;
            
            if (holder is not null) holder.Context = null;
            if (value is not null) Holder.Value = new ContextHolder { Context = value };
        }
    }

    // TODO - remove to other class.
    public void ChangeContext(IMessageContext messageContext, Type type)
    {
        if (messageContext is null) return;
        
        if(Context is null) Context = messageContext.Context;
        else Context.RequestId = messageContext.MessageId;
        
        Console.WriteLine($"{type.Name}: {Context.RequestId}");
    }

    private class ContextHolder
    {
        public IContext Context;
    }
}