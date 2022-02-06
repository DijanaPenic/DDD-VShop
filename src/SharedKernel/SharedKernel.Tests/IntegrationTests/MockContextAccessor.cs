using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Contexts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;

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

    private class ContextHolder
    {
        public IContext Context;
    }
}