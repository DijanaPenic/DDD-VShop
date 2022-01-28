using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Messaging;

internal static class MessagingExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddSingleton<IMessageContextProvider, MessageContextProvider>();
        services.AddSingleton<IMessageContextRegistry, MessageContextRegistry>();

        return services;
    }
}