namespace VShop.SharedKernel.Infrastructure.Contexts.Contracts;

public interface IContextAccessor
{
    public IContext Context { get; set; }
}