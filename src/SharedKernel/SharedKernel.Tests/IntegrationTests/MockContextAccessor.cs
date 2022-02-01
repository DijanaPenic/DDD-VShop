using VShop.SharedKernel.Infrastructure.Contexts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Types;

namespace VShop.SharedKernel.Tests.IntegrationTests;

public sealed class MockContextAccessor : IContextAccessor
{
    private static readonly AsyncLocal<ContextHolder> Holder = new();

    public IContext Context
    {
        get => Holder.Value?.Context ?? new Context
        (
            SequentialGuid.Create(),
            SequentialGuid.Create()
        );
        
        set
        {
            ContextHolder holder = Holder.Value;
            
            if (holder is not null) holder.Context = null;
            if (value is not null) Holder.Value = new ContextHolder { Context = value };
        }
    }
    public void ChangeContext(IMessageContext messageContext, Type type)
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