using System;
using System.Threading.Tasks;
using Serilog;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.EventSourcing;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.PostgresDb
{
    public class PostgresDbProjection<T> : ISubscription 
        where T : DbContext
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

        public async Task ProjectAsync(object @event)
        {
            Func<Task> handler = _projector(_dbContext, @event);
            
            if (handler == null) return;
            
            Logger.Debug("Projecting {Event}", @event);

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