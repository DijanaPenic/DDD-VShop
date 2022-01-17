using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace VShop.SharedKernel.Infrastructure.Services
{
    public sealed class DatabaseInitializerHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseInitializerHostedService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Type> dbContextTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(DbContext).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && t != typeof(DbContext));
            
            using IServiceScope scope = _serviceProvider.CreateScope();
            foreach (Type dbContextType in dbContextTypes)
            {
                DbContext dbContext = scope.ServiceProvider.GetService(dbContextType) as DbContext;
                if (dbContext is null) continue;

                await dbContext.Database.MigrateAsync(cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}