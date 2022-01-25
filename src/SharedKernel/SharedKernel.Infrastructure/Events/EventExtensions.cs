using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.Infrastructure.Events;

internal static class EventExtensions
{
    public static IServiceCollection AddEvents(this IServiceCollection services)
    {
        services.AddSingleton<IEventDispatcher, EventDispatcher>();
        
        return services;
    }
}