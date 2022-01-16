using Serilog;
using EventStore.Client;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

using VShop.Modules.Catalog.API.Application;
using VShop.SharedKernel.Integration.Stores;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.Integration.Services;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.SharedKernel.Integration.Projections;
using VShop.SharedKernel.EventStoreDb.Subscriptions;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Services;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Services.Contracts;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;

namespace VShop.Modules.Catalog.API.Infrastructure.Extensions
{
    public static class IntegrationExtensions
    {
        public static void AddIntegrationServices(this IServiceCollection services, string connectionString)
        {
            services.AddTransient<IIntegrationEventLogStore, IntegrationEventLogStore>();
            services.AddTransient<IIntegrationEventService, IntegrationEventService>();
            
            EventStoreClientSettings eventStoreSettings = EventStoreClientSettings.Create(connectionString);
            eventStoreSettings.ConnectionName = "Catalog";
            
            EventStoreClient eventStoreClient = new(eventStoreSettings);
            services.AddSingleton(eventStoreClient);
            
            services.AddSingleton
            (typeof(IIntegrationEventStore),
                typeof(IntegrationEventStore)
            );

            // NOTE: Cannot use AddHostedService to register individual workers of the same type.
            // Source: https://github.com/dotnet/runtime/issues/38751
            services.AddHostedService<SubscriptionHostedService>();

            // Subscribe to integration streams
            services.AddSingleton<ISubscriptionBackgroundService, SubscriptionToAllBackgroundService>(provider =>
            {
                ILogger logger = provider.GetService<ILogger>();
                return new SubscriptionToAllBackgroundService
                (
                    logger,
                    eventStoreClient,
                    provider,
                    new SubscriptionConfig
                    (
                        "IntegrationEventsSub",
                        new IntegrationEventPublisher
                        (
                            logger,
                            provider,
                            provider.GetRequiredService<IEventBus>()
                        ),
                        new SubscriptionFilterOptions(StreamFilter.RegularExpression
                            (new Regex(@".*\/integration$")))
                    )
                );
            });

            MessageMappings.Initialize();
        }
    }
}