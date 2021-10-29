using System;
using System.Threading.Tasks;
using Serilog;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Messaging;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.PostgresDb
{
    public class PostgresDbProjection<TDbContext> : ISubscription 
        where TDbContext : ApplicationDbContextBase
    {
        private readonly TDbContext _dbContext;
        private readonly Projector _projector;
        
        private static readonly ILogger Logger = Log.ForContext<PostgresDbProjection<TDbContext>>(); 
        
        public PostgresDbProjection
        (
            TDbContext dbContext,
            Projector projector
        )
        {
            _dbContext = dbContext;
            _projector = projector;
        }

        public async Task ProjectAsync(IDomainEvent eventData, EventMetadata eventMetadata)
        {
            Func<Task> handler = _projector(_dbContext, eventData);
            
            if (handler == null) return;
            
            Logger.Debug("Projecting {EventData}", eventData);

            await handler();
            await _dbContext.SaveChangesAsync(eventMetadata.EffectiveTime);
        }
        
        public delegate Func<Task> Projector
        (
            TDbContext dbContext,
            IDomainEvent eventData
        );
    }
}