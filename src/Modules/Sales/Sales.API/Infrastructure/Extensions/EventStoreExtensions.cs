using EventStore.ClientAPI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

using VShop.SharedKernel.EventStore;
using VShop.SharedKernel.PostgresDb.Projections;
using VShop.SharedKernel.EventStore.Projections;
using VShop.SharedKernel.EventStore.Repositories;
using VShop.SharedKernel.EventStore.Repositories.Contracts;
using VShop.SharedKernel.EventStore.Subscriptions;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.API.Projections;

namespace VShop.Modules.Sales.API.Infrastructure.Extensions
{
    public static class EventStoreExtensions
    {
        public static void AddEventStoreServices(this IServiceCollection services, string connectionString)
        {
            IEventStoreConnection esConnection = EventStoreConnection.Create
            (
                connectionString,
                ConnectionSettings.Create().KeepReconnecting().DisableTls(),
                "Sales"
            );

            services.AddSingleton(esConnection);
            services.AddSingleton(typeof(IEventStoreAggregateRepository<,>), typeof(EventStoreAggregateRepository<,>));
            services.AddSingleton(typeof(IEventStoreIntegrationRepository), typeof(EventStoreIntegrationRepository));
            services.AddSingleton<IHostedService, EventStoreService>();
            
            // Read model projections
            services.AddSingleton(provider =>
            {
                SalesContext dbContext = provider.GetRequiredService<SalesContext>();
                
                const string esSubscriptionName = "ReadModels";

                return new EventStoreAllCatchUpSubscriptionManager
                (
                    esConnection,
                    esSubscriptionName,
                    new DomainEventProjectionToPostgres<SalesContext>(dbContext, ShoppingCartInfoProjection.ProjectAsync)
                );
            });
            
            // Publish integration events from the current bounded context 
            services.AddSingleton(provider =>
            {
                IEventStoreIntegrationRepository integrationRepository = provider.GetRequiredService<IEventStoreIntegrationRepository>();
                
                const string esSubscriptionName = "IntegrationEventsPub";

                return new EventStoreAllFilteredCatchUpSubscriptionManager
                (
                    esConnection,
                    esSubscriptionName,
                    Filter.StreamId.Prefix(esConnection.ConnectionName),
                new IntegrationEventProjectionToEventStore(integrationRepository)
                );
            });
            
            // Subscribe to all integration streams
            services.AddSingleton(provider =>
            {
                Publisher publisher = provider.GetRequiredService<Publisher>();
                Regex regexExpression = new(@".*\/integration$");
                
                const string esSubscriptionName = "IntegrationEventsSub";

                return new EventStoreAllFilteredCatchUpSubscriptionManager
                (
                    esConnection,
                    esSubscriptionName,
                    Filter.StreamId.Regex(regexExpression),
                    new IntegrationEventProjectionPublisher(publisher)
                );
            });

            EventMappings.MapEventTypes();
        }
    }
}