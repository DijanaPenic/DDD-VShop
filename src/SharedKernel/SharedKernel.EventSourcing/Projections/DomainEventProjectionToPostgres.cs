using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.EventStoreDb.Messaging;
using VShop.SharedKernel.EventStoreDb.Subscriptions;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.EventSourcing.Projections
{
    // TODO - correct project for this class?
    public class DomainEventProjectionToPostgres<TDbContext> : ISubscriptionHandler 
        where TDbContext : DbContextBase
    {
        private readonly Projector _projector;
        
        private static readonly ILogger Logger = Log.ForContext<DomainEventProjectionToPostgres<TDbContext>>(); 
        
        public DomainEventProjectionToPostgres(Projector projector) => _projector = projector;

        public async Task ProjectAsync
        (
            IMessage message,
            IMessageMetadata metadata,
            IServiceScope scope,
            IDbContextTransaction transaction,
            CancellationToken cancellationToken = default
        )
        {
            if(message is not IDomainEvent domainEvent) return;
            
            TDbContext dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

            Func<Task> handler = _projector(dbContext, domainEvent);
        
            if (handler is null) return;
        
            Logger.Debug("Projecting domain event: {Message}", domainEvent);

            await handler();
            
            await dbContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);
            await dbContext.SaveChangesAsync(metadata.EffectiveTime, cancellationToken);
        }
        
        public delegate Func<Task> Projector
        (
            TDbContext dbContext,
            IDomainEvent @event
        );
    }
}