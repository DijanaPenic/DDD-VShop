using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using NodaTime.Serialization.Protobuf;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.EventStoreDb.Subscriptions;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure;

namespace VShop.SharedKernel.Application.Projections
{
    public class DomainEventProjectionToPostgres<TDbContext> : ISubscriptionHandler where TDbContext : DbContextBase
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly Projector _projector;

        public DomainEventProjectionToPostgres
        (
            ILogger logger,
            IServiceProvider serviceProvider,
            Projector projector
        )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _projector = projector;
        }

        public async Task ProjectAsync
        (
            ResolvedEvent resolvedEvent,
            Func<SubscriptionDbContext, Task> checkpointUpdate,
            CancellationToken cancellationToken = default
        )
        {
            IDomainEvent domainEvent = resolvedEvent.Deserialize<IDomainEvent>();
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
        
                _logger.Debug("Projecting domain event: {Message}", domainEvent);

                await handler();
                
                await readDataContext.SaveChangesAsync(domainEvent.Metadata.EffectiveTime.ToInstant(), cancellationToken);

                await subscriptionDbContext.Database.UseTransactionAsync
                (
                    readDataContext.CurrentTransaction.GetDbTransaction(),
                    cancellationToken
                );
                
                await checkpointUpdate(subscriptionDbContext);
                
                // Read model and checkpoint update will be committed in the same transaction.
                await readDataContext.CommitCurrentTransactionAsync(cancellationToken);
            });
        }
        
        public delegate Func<Task> Projector(TDbContext dbContext, IDomainEvent @event);
    }
}