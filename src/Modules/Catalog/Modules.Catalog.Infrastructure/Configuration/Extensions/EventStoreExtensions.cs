using Serilog;
using EventStore.Client;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.EventStoreDb.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Integration.Projections;
using VShop.SharedKernel.Integration.Services;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.SharedKernel.Integration.Stores;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.Services;
using VShop.SharedKernel.Subscriptions.Services.Contracts;

[assembly: InternalsVisibleTo("VShop.Modules.Catalog.API")]
namespace VShop.Modules.Catalog.Infrastructure.Configuration.Extensions
{
    internal static class EventStoreExtensions
    {
        public static void AddEventStore(this IServiceCollection services, string connectionString)
        {
            services.AddTransient<IIntegrationEventOutbox, IntegrationEventOutbox>();
            services.AddTransient<IIntegrationEventService, IntegrationEventService>();
            services.AddEventStoreInfrastructure(connectionString, "Catalog");

            services.AddSingleton
            (
                typeof(IIntegrationEventStore),
                typeof(IntegrationEventStore)
            );

            // Subscribe to integration streams
            services.AddSingleton<IntegrationEventPublisher>();
            services.AddSingleton<IEventStoreBackgroundService, EventStoreBackgroundService>(provider =>
            {
                EventStoreClient eventStoreClient = provider.GetService<EventStoreClient>();
                ILogger logger = provider.GetService<ILogger>();
                IMessageRegistry messageRegistry = provider.GetService<IMessageRegistry>();
                IEventStoreMessageConverter eventStoreMessageConverter = provider.GetService<IEventStoreMessageConverter>();
                IMessageContextRegistry messageContextRegistry = provider.GetService<IMessageContextRegistry>();
                ISubscriptionHandler subscriptionHandler = provider.GetService<IntegrationEventPublisher>();

                return new EventStoreBackgroundService
                (
                    logger, eventStoreClient, provider, messageRegistry, 
                    eventStoreMessageConverter, messageContextRegistry,
                    new SubscriptionConfig
                    (
                        "IntegrationEventsSub",
                        subscriptionHandler,
                        new SubscriptionFilterOptions(StreamFilter.RegularExpression
                            (new Regex(@".*\/integration$")))
                    )
                );
            });
        }
    }
}