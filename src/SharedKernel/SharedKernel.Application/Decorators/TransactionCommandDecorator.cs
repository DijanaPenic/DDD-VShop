﻿using MediatR;
using Serilog;
using Serilog.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;

namespace VShop.SharedKernel.Application.Decorators
{
    public class TransactionCommandDecorator<TCommand, TResponse> : ICommandDecorator<TCommand, TResponse>
        where TCommand : IRequest<TResponse>
        where TResponse : IResult
    {
        private readonly ILogger _logger;
        private readonly DbContextBase _dbContext;
        private readonly IIntegrationEventService _integrationEventService;

        public TransactionCommandDecorator
        (
            ILogger logger,
            MainDbContextProvider dbContextProvider,
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
                // Handling retries
                if (_dbContext.HasActiveTransaction) await _dbContext.DisposeCurrentTransactionAsync();
                
                TResponse response = default;

                // Source: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency#execution-strategies-and-transactions
                IExecutionStrategy strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;

                    // All messages written during a transaction will carry the id of that transaction
                    await using (IDbContextTransaction transaction = await _dbContext.BeginTransactionAsync(cancellationToken))
                    using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
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

                        await _dbContext.CommitCurrentTransactionAsync(cancellationToken);

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
}