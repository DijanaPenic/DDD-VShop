using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Contexts;

public class ContextAdapter : IContextAdapter
{
    private readonly IContextAccessor _contextAccessor;
    private readonly IMessageContextRegistry _messageContextRegistry;

    public ContextAdapter(IContextAccessor contextAccessor, IMessageContextRegistry messageContextRegistry)
    {
        _contextAccessor = contextAccessor;
        _messageContextRegistry = messageContextRegistry;
    }

    public void ChangeContext(IMessage message)
    {
        IMessageContext messageContext = _messageContextRegistry.Get(message);
        if (messageContext is null) return;
        
        if(_contextAccessor.Context is null) _contextAccessor.Context = messageContext.Context;
        else _contextAccessor.Context.RequestId = messageContext.MessageId;
    }
}