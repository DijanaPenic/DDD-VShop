using System.Threading;

using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Contexts;

public sealed class ContextAccessor
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

    public void ChangeContext(IMessageContext messageContext)
    {
        if (messageContext is null) return;
        
        if(Context is null) Context = messageContext.Context;
        else Context.RequestId = messageContext.MessageId;
    }

    private class ContextHolder
    {
        public IContext Context;
    }
}