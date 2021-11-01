using System;
using System.Threading.Tasks;
using Serilog;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Messaging;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.PostgresDb.Projections
{
    public class PostgresDomainProjection<TDbContext> : ISubscription 
        where TDbContext : ApplicationDbContextBase
    {
        private readonly TDbContext _dbContext;
        private readonly Projector _projector;
        
        private static readonly ILogger Logger = Log.ForContext<PostgresDomainProjection<TDbContext>>(); 
        
        public PostgresDomainProjection
        (
            TDbContext dbContext,
            Projector projector
        )
        {
            _dbContext = dbContext;
            _projector = projector;
        }

        public async Task ProjectAsync(IMessage message, MessageMetadata metadata)
        {
            if(message is IDomainEvent domainEvent)
            {
                Func<Task> handler = _projector(_dbContext, domainEvent);
            
                if (handler == null) return;
            
                Logger.Debug("Projecting domain event: {Message}", domainEvent);

                await handler();
                await _dbContext.SaveChangesAsync(metadata.EffectiveTime);
            }
        }
        
        public delegate Func<Task> Projector
        (
            TDbContext dbContext,
            IMessage eventData
        );
    }
}