using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace VShop.SharedKernel.Infrastructure.Services
{
    public sealed class DatabaseInitializerHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public DatabaseInitializerHostedService(IServiceProvider serviceProvider, ILogger logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Starting database initializer hosted service");
            
            IEnumerable<Type> dbContextTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(DbContext).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && t != typeof(DbContext));
            
            using IServiceScope scope = _serviceProvider.CreateScope();
            foreach (Type dbContextType in dbContextTypes)
            {
                if (scope.ServiceProvider.GetService(dbContextType) is not DbContext dbContext) continue;

                await dbContext.Database.MigrateAsync(cancellationToken);

                _logger.Information
                (
                    "Finished database migration for: {TypeName}",
                    dbContext.GetType().Name
                );
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Stopping database initializer hosted service");
            
            return Task.CompletedTask;
        }
    }
}