using System;
using System.Collections.Generic;
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

    public void Set<TMessage>(IEnumerable<MessageEnvelope<TMessage>> messages) where TMessage : IMessage
    {
        foreach ((IMessage message, IMessageContext messageContext) in messages)
            Set(message, messageContext);
    }
    
    public IMessageContext Get(IMessage message) => _cache.Get<IMessageContext>(message);
}