using Serilog;
using EventStore.Client;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.EventStoreDb;
using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.Services;
using VShop.SharedKernel.Integration.Stores;
using VShop.SharedKernel.Integration.Services;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.SharedKernel.Integration.Projections;
using VShop.SharedKernel.Integration.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

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
            
            // NOTE: Cannot use AddHostedService to register individual workers of the same type.
            // Source: https://github.com/dotnet/runtime/issues/38751
            services.AddHostedService<EventStoreSubscriptionHostedService>();

            // Subscribe to integration streams
            services.AddSingleton<ISubscriptionBackgroundService, EventStoreSubscriptionBackgroundService>(provider =>
            {
                ILogger logger = provider.GetService<ILogger>();
                IMessageRegistry messageRegistry = provider.GetService<IMessageRegistry>();
                
                return new EventStoreSubscriptionBackgroundService
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