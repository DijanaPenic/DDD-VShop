using System.Threading;

using VShop.SharedKernel.Infrastructure.Contexts.Contracts;

namespace VShop.SharedKernel.Infrastructure.Contexts;

public sealed class ContextAccessor
{
    private static readonly AsyncLocal<ContextHolder> Holder = new();

    public static IContext Context
    {
        get => Holder.Value?.Context;
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