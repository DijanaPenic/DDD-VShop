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
using VShop.SharedKernel.EventStoreDb.Messaging;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.EventStoreDb.Subscriptions;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure;

namespace VShop.SharedKernel.Application.Projections
{
    public class DomainEventProjectionToPostgres<TDbContext> : ISubscriptionHandler 
        where TDbContext : DbContextBase
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
            IMessage message = resolvedEvent.DeserializeData<IMessage>();
            if(message is not IDomainEvent domainEvent) return;

            using IServiceScope scope = _serviceProvider.CreateScope();
            TDbContext readDataContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            SubscriptionContext subscriptionContext = scope.ServiceProvider.GetRequiredService<SubscriptionContext>();
                
            IExecutionStrategy strategy = subscriptionContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using IDbContextTransaction transaction = await subscriptionContext.BeginTransactionAsync(cancellationToken);
                
                Func<Task> handler = _projector(readDataContext, domainEvent);
        
                if (handler is null) return;
        
                _logger.Debug("Projecting domain event: {Message}", domainEvent);

                await handler();

                await readDataContext.Database.UseTransactionAsync
                (
                    subscriptionContext.CurrentTransaction.GetDbTransaction(),
                    cancellationToken
                );
                
                IMessageMetadata metadata = resolvedEvent.DeserializeMetadata();
                await readDataContext.SaveChangesAsync(metadata.EffectiveTime, cancellationToken);
                
                await checkpointUpdate(subscriptionContext);
                
                await subscriptionContext.CommitTransactionAsync(transaction, cancellationToken);
            });
        }
        
        public delegate Func<Task> Projector(TDbContext dbContext, IDomainEvent @event);
    }
}