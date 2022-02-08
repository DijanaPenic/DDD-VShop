using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.EventStoreDb.Messaging;
using VShop.SharedKernel.EventStoreDb.Messaging.Contracts;
using VShop.SharedKernel.EventStoreDb.Serialization;
using VShop.SharedKernel.EventStoreDb.Serialization.Contracts;

namespace VShop.SharedKernel.EventStoreDb.Extensions;

public static class EventStoreExtensions
{
    public static IServiceCollection AddEventStoreInfrastructure
    (
        this IServiceCollection services,
        string connectionString,
        string connectionName
    )
    {
        EventStoreClientSettings eventStoreSettings = EventStoreClientSettings.Create(connectionString);
        eventStoreSettings.ConnectionName = connectionName;

        services.AddSingleton(new EventStoreClient(eventStoreSettings));
        services.AddSingleton<CustomEventStoreClient>();
        services.AddSingleton<IEventStoreSerializer, EventStoreProtobufSerializer>();
        services.AddSingleton<IEventStoreMessageConverter, EventStoreMessageConverter>();

        return services;
    }
}