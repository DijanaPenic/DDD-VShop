using System;
using System.Threading.Tasks;
using Serilog;

using VShop.SharedKernel.EventSourcing;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.PostgresDb
{
    public class PostgresDbProjection<T> : ISubscription 
        where T : ApplicationDbContextBase
    {
        private readonly T _dbContext;
        private readonly Projector _projector;
        
        private static readonly ILogger Logger = Log.ForContext<PostgresDbProjection<T>>(); 
        
        public PostgresDbProjection
        (
            T dbContext,
            Projector projector
        )
        {
            _dbContext = dbContext;
            _projector = projector;
        }

        public async Task ProjectAsync(object eventData, EventMetadata eventMetadata)
        {
            Func<Task> handler = _projector(_dbContext, eventData);
            
            if (handler == null) return;
            
            Logger.Debug("Projecting {EventData}", eventData);

            await handler();
            await _dbContext.SaveChangesAsync(eventMetadata.EffectiveTime);
        }
        
        public delegate Func<Task> Projector
        (
            T dbContext,
            object eventData
        );
    }
}