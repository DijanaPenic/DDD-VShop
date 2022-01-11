using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.PostgresDb;
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
            Func<SubscriptionContext, Task> checkpointUpdate,
            CancellationToken cancellationToken = default
        )
        {
            IIdentifiedMessage<IMessage> message = resolvedEvent.Deserialize<IMessage>();
            if(message.Data is not IDomainEvent domainEvent) return;

            using IServiceScope scope = _serviceProvider.CreateScope();
            TDbContext readDataContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            SubscriptionContext subscriptionContext = scope.ServiceProvider.GetRequiredService<SubscriptionContext>();
            
            IExecutionStrategy strategy = readDataContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await readDataContext.BeginTransactionAsync(cancellationToken);
                
                Func<Task> handler = _projector(readDataContext, domainEvent);
        
                if (handler is null) return;
        
                _logger.Debug("Projecting domain event: {Message}", domainEvent);

                await handler();
                
                // TODO - convert.
                //await readDataContext.SaveChangesAsync(message.Metadata.EffectiveTime, cancellationToken);
                await readDataContext.SaveChangesAsync(cancellationToken);
                
                await subscriptionContext.Database.UseTransactionAsync
                (
                    readDataContext.CurrentTransaction.GetDbTransaction(),
                    cancellationToken
                );
                
                await checkpointUpdate(subscriptionContext);
                
                // Read model and checkpoint update will be committed in the same transaction.
                await readDataContext.CommitCurrentTransactionAsync(cancellationToken);
            });
        }
        
        public delegate Func<Task> Projector(TDbContext dbContext, IDomainEvent @event);
    }
}