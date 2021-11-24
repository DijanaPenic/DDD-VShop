using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.EventSourcing.Messaging;
using VShop.SharedKernel.EventSourcing.Projections;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.PostgresDb.Projections
{
    public class DomainEventProjectionToPostgres<TDbContext> : ISubscription 
        where TDbContext : DbContextBase
    {
        private readonly IServiceProvider _services;
        private readonly Projector _projector;
        
        private static readonly ILogger Logger = Log.ForContext<DomainEventProjectionToPostgres<TDbContext>>(); 
        
        public DomainEventProjectionToPostgres
        (
            IServiceProvider services,
            Projector projector
        )
        {
            _services = services;
            _projector = projector;
        }

        public async Task ProjectAsync(IMessage message, IMessageMetadata metadata, CancellationToken cancellationToken)
        {
            if(message is IDomainEvent domainEvent)
            {
                // Consuming a scoped service in a background task
                // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-5.0&tabs=visual-studio#consuming-a-scoped-service-in-a-background-task-1
                using IServiceScope scope = _services.CreateScope();
                TDbContext dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

                Func<Task> handler = _projector(dbContext, domainEvent);
            
                if (handler is null) return;
            
                Logger.Debug("Projecting domain event: {Message}", domainEvent);

                await handler();
                await dbContext.SaveChangesAsync(metadata.EffectiveTime, cancellationToken);
            }
        }
        
        public delegate Func<Task> Projector
        (
            TDbContext dbContext,
            IDomainEvent @event
        );
    }
}