﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using Serilog.Context;

using VShop.Modules.Billing.Infrastructure;
using VShop.Modules.Billing.Integration.Services;
using VShop.SharedKernel.Application.Decorators;

using ILogger = Serilog.ILogger;

namespace VShop.Modules.Billing.API.Application.Decorators
{
    // TODO - create decorator template class
    public class TransactionCommandDecorator<TCommand, TResponse> : ICommandDecorator<TCommand, TResponse>
    {
        private readonly BillingContext _billingContext;
        private readonly IBillingIntegrationEventService _billingIntegrationEventService;
        
        private static readonly ILogger Logger = Log.ForContext<TransactionCommandDecorator<TCommand, TResponse>>(); 

        public TransactionCommandDecorator
        (
            BillingContext billingContext,
            IBillingIntegrationEventService billingIntegrationEventService
        )
        {
            _billingContext = billingContext;
            _billingIntegrationEventService = billingIntegrationEventService;
        }

        public async Task<TResponse> Handle
        (
            TCommand command,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next
        )
        {
            TResponse response = default;
            string commandTypeName = command.GetType().FullName;

            try
            {
                // Handling retries
                if (_billingContext.HasActiveTransaction) return await next();

                IExecutionStrategy strategy = _billingContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;

                    await using (IDbContextTransaction transaction = await _billingContext.BeginTransactionAsync())
                    using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
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

                        await _billingContext.CommitTransactionAsync(transaction, cancellationToken);

                        transactionId = transaction.TransactionId;
                    }

                    await _billingIntegrationEventService.PublishEventsAsync(transactionId, cancellationToken);
                });

                return response;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error Handling transaction for {CommandName} ({@Command})", commandTypeName, command);

                throw;
            }
        }
    }
}