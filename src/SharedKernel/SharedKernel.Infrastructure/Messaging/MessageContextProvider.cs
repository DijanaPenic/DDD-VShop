using Microsoft.Extensions.Caching.Memory;

using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Messaging;

public class MessageContextProvider : IMessageContextProvider
{
    private readonly IMemoryCache _cache;

    public MessageContextProvider(IMemoryCache cache) => _cache = cache;

    public IMessageContext Get(IMessage message) => _cache.Get<IMessageContext>(message);
}