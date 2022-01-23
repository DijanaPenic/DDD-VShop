using Microsoft.Extensions.DependencyInjection;

namespace VShop.SharedKernel.Infrastructure.Messaging;

public static class MessagingExtensions
{
    private const string SectionName = "messaging";
        
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        //services.AddTransient<IMessageBroker, InMemoryMessageBroker>();
        //services.AddSingleton<IMessageContextProvider, MessageContextProvider>(); // cache
        //services.AddSingleton<IMessageContextRegistry, MessageContextRegistry>(); // cache

        return services;
    }
}