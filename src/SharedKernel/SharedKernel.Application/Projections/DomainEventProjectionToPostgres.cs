using Serilog;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using NodaTime.Serialization.Protobuf;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.DAL;
using VShop.SharedKernel.EventStoreDb.Messaging;
using VShop.SharedKernel.EventStoreDb.Messaging.Contracts;
using VShop.SharedKernel.EventStoreDb.Serialization.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.Application.Projections
{
    public class DomainEventProjectionToPostgres<TDbContext> : ISubscriptionHandler where TDbContext : DbContextBase
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventStoreMessageConverter _eventStoreMessageConverter;
        private readonly IEventStoreSerializer _eventStoreSerializer;
        private readonly Projector _projector;

        public DomainEventProjectionToPostgres
        (
            ILogger logger,
            IServiceProvider serviceProvider,
            IEventStoreMessageConverter eventStoreMessageConverter,
            IEventStoreSerializer eventStoreSerializer,
            Projector projector
        )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _eventStoreMessageConverter = eventStoreMessageConverter;
            _eventStoreSerializer = eventStoreSerializer;
            _projector = projector;
        }

        public async Task ProjectAsync
        (
            ResolvedEvent resolvedEvent,
            Func<SubscriptionDbContext, Task> checkpointUpdate,
            CancellationToken cancellationToken = default
        )
        {
            IDomainEvent domainEvent = _eventStoreMessageConverter.ToMessage<IDomainEvent>(resolvedEvent)?.Message;
            if(domainEvent is null) return;

            using IServiceScope scope = _serviceProvider.CreateScope();
            TDbContext readDataContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            SubscriptionDbContext subscriptionDbContext = scope.ServiceProvider.GetRequiredService<SubscriptionDbContext>();
            
            IExecutionStrategy strategy = readDataContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await readDataContext.BeginTransactionAsync(cancellationToken);
                
                Func<Task> handler = _projector(readDataContext, domainEvent);
        
                if (handler is null) return;

                _logger.Debug
                (
                    "Projecting domain event {Type}: {@Message}",
                    domainEvent.GetType().Name, domainEvent
                );

                await handler();
                
                MessageMetadata messageMetadata = _eventStoreSerializer.Deserialize
                    <MessageMetadata>(resolvedEvent.Event.Metadata.Span.ToArray());
                
                await readDataContext.SaveChangesAsync(messageMetadata.EffectiveTime.ToInstant(), cancellationToken);

                await subscriptionDbContext.Database.UseTransactionAsync
                (
                    readDataContext.CurrentTransaction.GetDbTransaction(),
                    cancellationToken
                );
                
                await checkpointUpdate(subscriptionDbContext);
                
                // Read model and checkpoint update will be committed in the same transaction.
                await readDataContext.CommitTransactionAsync(cancellationToken);
            });
        }
        
        public delegate Func<Task> Projector(TDbContext dbContext, IDomainEvent @event);
    }
}