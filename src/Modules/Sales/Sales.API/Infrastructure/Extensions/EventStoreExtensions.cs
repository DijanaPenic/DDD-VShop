using EventStore.ClientAPI;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.EventStore;
using VShop.SharedKernel.PostgresDb.Projections;
using VShop.SharedKernel.EventStore.Projections;
using VShop.SharedKernel.EventStore.Repositories;
using VShop.SharedKernel.EventStore.Repositories.Contracts;
using VShop.SharedKernel.EventStore.Subscriptions;
using VShop.SharedKernel.EventStore.Subscriptions.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.API.Projections;

namespace VShop.Modules.Sales.API.Infrastructure.Extensions
{
    public static class EventStoreExtensions
    {
        public static void AddEventStoreServices(this IServiceCollection services, string connectionString)
        {
            // TODO - switch to gRPC
            IEventStoreConnection esConnection = EventStoreConnection.Create
            (
                connectionString,
                ConnectionSettings.Create().KeepReconnecting().DisableTls(),
                "Sales"
            );

            services.AddSingleton(esConnection);
            services.AddSingleton(typeof(IEventStoreAggregateRepository<,>), typeof(EventStoreAggregateRepository<,>));
            services.AddSingleton(typeof(IEventStoreIntegrationRepository), typeof(EventStoreIntegrationRepository));
            services.AddHostedService<EventStoreService>();
            
            // Read model projections
            services.AddSingleton<IEventStoreSubscriptionManager, EventStoreAllCatchUpSubscriptionManager>(provider => new EventStoreAllCatchUpSubscriptionManager
            (
                esConnection,
                "ReadModels",
                new DomainEventProjectionToPostgres<SalesContext>(provider, ShoppingCartInfoProjection.ProjectAsync)
            ));
            
            // Publish integration events from the current bounded context 
            services.AddSingleton<IEventStoreSubscriptionManager, EventStoreAllFilteredCatchUpSubscriptionManager>(provider =>
            {
                IEventStoreIntegrationRepository integrationRepository = provider.GetRequiredService<IEventStoreIntegrationRepository>();

                return new EventStoreAllFilteredCatchUpSubscriptionManager
                (
                    esConnection,
                    "IntegrationEventsPub",
                    Filter.StreamId.Prefix($"{esConnection.ConnectionName}/aggregate".ToSnakeCase()),
                new IntegrationEventProjectionToEventStore(integrationRepository)
                );
            });
            
            // Subscribe to all integration streams
            services.AddSingleton<IEventStoreSubscriptionManager, EventStoreAllFilteredCatchUpSubscriptionManager>(provider =>
            {
                Publisher publisher = provider.GetRequiredService<Publisher>();

                return new EventStoreAllFilteredCatchUpSubscriptionManager
                (
                    esConnection,
                    "IntegrationEventsSub",
                    Filter.StreamId.Regex(new Regex(@".*\/integration$")),
                    new IntegrationEventProjectionPublisher(publisher)
                );
            });

            EventMappings.MapEventTypes();
        }
    }
}