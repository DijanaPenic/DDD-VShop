using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Messaging;

public static class MessagingExtensions
{
    internal static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddSingleton<IMessageContextProvider, MessageContextProvider>();
        services.AddSingleton<IMessageContextRegistry, MessageContextRegistry>();

        return services;
    }

    public static IReadOnlyList<TMessage> ToMessages<TMessage>(this IEnumerable<MessageEnvelope<TMessage>> messages)
        where TMessage : IMessage
        => messages.Select(e => e.Message).ToList();
}