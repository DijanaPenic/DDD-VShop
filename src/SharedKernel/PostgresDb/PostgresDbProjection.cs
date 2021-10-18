using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.EventSourcing;

namespace VShop.SharedKernel.PostgresDb
{
    public class PostgresDbProjection<T> : ISubscription 
        where T : DbContext
    {
        // TODO - enable logging
        //static readonly ILog Log = LogProvider.GetCurrentClassLogger();
        
        private readonly T _dbContext;
        private readonly Projector _projector;
        
        public PostgresDbProjection
        (
            T dbContext,
            Projector projector
        )
        {
            _dbContext = dbContext;
            _projector = projector;
        }

        public async Task ProjectAsync(object @event)
        {
            Func<Task> handler = _projector(_dbContext, @event);
            
            if (handler == null) return;
            
            //Log.Debug("Projecting {event}.", @event);

            await handler();
            await _dbContext.SaveChangesAsync();
        }
        
        public delegate Func<Task >Projector
        (
            T dbContext,
            object @event
        );
    }
}