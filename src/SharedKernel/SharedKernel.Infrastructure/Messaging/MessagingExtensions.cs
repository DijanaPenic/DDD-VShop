using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Messaging;

public static class MessagingExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
        IMessageContextRegistry messageContextRegistry = new MessageContextRegistry(memoryCache);
        services.AddSingleton(messageContextRegistry);

        return services;
    }

    public static IReadOnlyList<TMessage> ToMessages<TMessage>(this IEnumerable<MessageEnvelope<TMessage>> messages)
        where TMessage : IMessage
        => messages.Select(e => e.Message).ToList();
}