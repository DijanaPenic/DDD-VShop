using System;
using Microsoft.Extensions.Caching.Memory;

using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Messaging;

public class MessageContextRegistry : IMessageContextRegistry
{
    private readonly IMemoryCache _cache;

    public MessageContextRegistry(IMemoryCache cache) => _cache = cache;

    public void Set<TMessage>(TMessage message, IMessageContext context) where TMessage : IMessage
        => _cache.Set(message, context, new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(1)
        });
}