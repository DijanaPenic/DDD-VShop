using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using VShop.SharedKernel.PostgresDb.Contracts;

namespace VShop.SharedKernel.PostgresDb;

public class PostgresUnitOfWork<TDbContext> : IUnitOfWork where TDbContext : DbContextBase
{
    private readonly TDbContext _dbContext;
    private readonly ILogger _logger;
    public IDbContextTransaction CurrentTransaction => _dbContext.CurrentTransaction;

    protected PostgresUnitOfWork(TDbContext dbContext, ILogger logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Guid> ExecuteAsync
    (
        Func<Task> action,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            // Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            // Source: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            IExecutionStrategy strategy = _dbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using IDbContextTransaction transaction = await _dbContext.BeginTransactionAsync(cancellationToken);
                Guid transactionId = transaction.TransactionId;
            
                await action();
                await _dbContext.CommitTransactionAsync(cancellationToken);

                return transactionId;
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unhandled error has occurred");
            await _dbContext.RollbackTransactionAsync(cancellationToken);
            
            throw;
        }
    }

    public async Task<(Guid, TResponse)> ExecuteAsync<TResponse>
    (
        Func<Task<TResponse>> action,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            IExecutionStrategy strategy = _dbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using IDbContextTransaction transaction = await _dbContext.BeginTransactionAsync(cancellationToken);
                Guid transactionId = transaction.TransactionId;
            
                TResponse response = await action();
                await _dbContext.CommitTransactionAsync(cancellationToken);

                return (transactionId, response);
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unhandled error has occurred");
            await _dbContext.RollbackTransactionAsync(cancellationToken);
            
            throw;
        }
    }
}