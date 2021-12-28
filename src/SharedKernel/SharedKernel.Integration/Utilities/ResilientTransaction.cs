using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using VShop.SharedKernel.PostgresDb;

namespace VShop.SharedKernel.Integration.Utilities
{
    public class ResilientTransaction
    {
        private readonly DbContextBase _dbContext;

        private ResilientTransaction(DbContextBase dbContext) => _dbContext = dbContext;

        public static ResilientTransaction New(DbContextBase context) => new(context);

        public async Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            // Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            // Source: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            IExecutionStrategy strategy = _dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using IDbContextTransaction transaction = await _dbContext.BeginTransactionAsync(cancellationToken);

                await action();
                await _dbContext.CommitCurrentTransactionAsync(cancellationToken);
            });
        }
    }
}