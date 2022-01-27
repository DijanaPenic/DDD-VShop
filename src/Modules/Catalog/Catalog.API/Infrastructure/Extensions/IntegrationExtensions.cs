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
using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.Services;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Subscriptions.Services.Contracts;

namespace VShop.Modules.Catalog.API.Infrastructure.Extensions
{
    public static class IntegrationExtensions
    {
        public static void AddIntegrationServices(this IServiceCollection services, string connectionString)
        {
            services.AddTransient<IIntegrationEventOutbox, IntegrationEventOutbox>();
            services.AddTransient<IIntegrationEventService, IntegrationEventService>();
            
            EventStoreClientSettings eventStoreSettings = EventStoreClientSettings.Create(connectionString);
            eventStoreSettings.ConnectionName = "Catalog";
            
            EventStoreClient eventStoreClient = new(eventStoreSettings);
            services.AddSingleton(eventStoreClient);
            
            services.AddSingleton
            (typeof(IIntegrationEventStore),
                typeof(IntegrationEventStore)
            );

            // Subscribe to integration streams
            services.AddSingleton<IEventStoreBackgroundService, EventStoreBackgroundService>(provider =>
            {
                ILogger logger = provider.GetService<ILogger>();
                IMessageRegistry messageRegistry = provider.GetService<IMessageRegistry>();
                
                return new EventStoreBackgroundService
                (
                    logger, eventStoreClient, provider, messageRegistry,
                    new SubscriptionConfig
                    (
                        "IntegrationEventsSub",
                        new IntegrationEventPublisher
                        (
                            logger, provider, messageRegistry,
                            provider.GetRequiredService<IEventDispatcher>()
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