using System.Threading;

using VShop.SharedKernel.Infrastructure.Contexts.Contracts;

namespace VShop.SharedKernel.Infrastructure.Contexts;

public sealed class ContextAccessor
{
    private static readonly AsyncLocal<ContextHolder> Holder = new();

    public static IRequestContext RequestContext
    {
        get => Holder.Value?.RequestContext;
        set
        {
            ContextHolder holder = Holder.Value;
            
            if (holder is not null) holder.RequestContext = null;
            if (value is not null) Holder.Value = new ContextHolder { RequestContext = value };
        }
    }

    private class ContextHolder
    {
        public IRequestContext RequestContext;
    }
}