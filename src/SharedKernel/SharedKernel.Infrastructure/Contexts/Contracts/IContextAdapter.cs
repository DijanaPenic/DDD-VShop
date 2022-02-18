using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Contexts.Contracts;

public interface IContextAdapter
{
    void ChangeContext(IMessage message);
}