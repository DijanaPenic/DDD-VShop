using Serilog;
using EventStore.Client;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.Services;
using VShop.SharedKernel.Subscriptions.Services.Contracts;
using VShop.SharedKernel.Integration.Stores;
using VShop.SharedKernel.Integration.Services;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.SharedKernel.Integration.Projections;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

[assembly: InternalsVisibleTo("VShop.Modules.Billing.API")]
namespace VShop.Modules.Billing.Infrastructure.Configuration.Extensions
{
    internal static class EventStoreExtensions
    {
        public static void AddEventStore(this IServiceCollection services, string connectionString)
        {
            services.AddTransient<IIntegrationEventOutbox, IntegrationEventOutbox>();
            services.AddTransient<IIntegrationEventService, IntegrationEventService>();

            EventStoreClientSettings eventStoreSettings = EventStoreClientSettings.Create(connectionString);
            eventStoreSettings.ConnectionName = "Billing";
            
            EventStoreClient eventStoreClient = new(eventStoreSettings);
            services.AddSingleton(eventStoreClient);
            
            services.AddSingleton<CustomEventStoreClient>();
            
            services.AddSingleton
            (typeof(IIntegrationEventStore),
                typeof(IntegrationEventStore)
            );

            // Subscribe to integration streams.
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
        }
    }
}