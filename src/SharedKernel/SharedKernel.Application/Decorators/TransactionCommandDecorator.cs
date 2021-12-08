using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;
using Serilog.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.SharedKernel.Application.Decorators.Contracts;

using ILogger = Serilog.ILogger;

namespace VShop.SharedKernel.Application.Decorators
{
    public class TransactionCommandDecorator<TCommand, TResponse> : ICommandDecorator<TCommand, TResponse>
    {
        private readonly DbContextBase _dbContext;
        private readonly IIntegrationEventService _integrationEventService;
        
        private static readonly ILogger Logger = Log.ForContext<TransactionCommandDecorator<TCommand, TResponse>>(); 

        public TransactionCommandDecorator
        (
            DbContextProvider dbContextProvider,
            IIntegrationEventService integrationEventService
        )
        {
            _dbContext = dbContextProvider();
            _integrationEventService = integrationEventService;
        }

        public async Task<TResponse> Handle
        (
            TCommand command,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next
        )
        {
            string commandTypeName = command.GetType().FullName;

            try
            {
                // TODO - need to test this. It might not work.
                // Handling retries
                if (_dbContext.HasActiveTransaction) return await next();
                
                TResponse response = default;

                // Source: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency#execution-strategies-and-transactions
                IExecutionStrategy strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;

                    await using (IDbContextTransaction transaction = await _dbContext.BeginTransactionAsync(cancellationToken))
                    using (LogContext.PushProperty("TransactionContext", transaction.TransactionId)) // All messages written during a transaction will carry the id of that transaction
                    {
                        Logger.Information
                        (
                            "Begin transaction {TransactionId} for {CommandName} ({@Command})",
                            transaction.TransactionId, commandTypeName, command
                        );

                        response = await next();

                        Logger.Information
                        (
                            "Commit transaction {TransactionId} for {CommandName}",
                            transaction.TransactionId, commandTypeName
                        );

                        await _dbContext.CommitTransactionAsync(transaction, cancellationToken);

                        transactionId = transaction.TransactionId;
                    }

                    await _integrationEventService.PublishEventsAsync(transactionId, cancellationToken);
                });

                return response;
            }
            catch (Exception ex)
            {
                Logger.Error
                (
                    ex, "Error Handling transaction for {CommandName} ({@Command})",
                    commandTypeName, command
                );

                throw;
            }
        }
    }
    
    public delegate DbContextBase DbContextProvider();
}