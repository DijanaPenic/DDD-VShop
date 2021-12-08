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

namespace VShop.SharedKernel.Application.Decorators
{
    public class TransactionCommandDecorator<TCommand, TResponse> : ICommandDecorator<TCommand, TResponse>
    {
        private readonly ILogger _logger;
        private readonly DbContextBase _dbContext;
        private readonly IIntegrationEventService _integrationEventService;

        public TransactionCommandDecorator
        (
            ILogger logger,
            DbContextProvider dbContextProvider,
            IIntegrationEventService integrationEventService
        )
        {
            _logger = logger;
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
                        _logger.Information
                        (
                            "Begin transaction {TransactionId} for {CommandName} ({@Command})",
                            transaction.TransactionId, commandTypeName, command
                        );

                        response = await next();

                        _logger.Information
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
                _logger.Error
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