using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using Serilog.Context;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Application.Decorators;
using VShop.Modules.Billing.Infrastructure;
using VShop.Modules.Billing.Integration.Services;

using ILogger = Serilog.ILogger;

namespace VShop.Modules.Billing.API.Application.Decorators
{
    // TODO - create decorator template class
    public class TransactionCommandDecorator<TRequest, TResponse> : ICommandDecorator<TRequest, TResponse>
    {
        // TODO - rename all context parameters
        private readonly BillingContext _dbContext;
        private readonly IBillingIntegrationEventService _billingIntegrationEventService;
        
        private static readonly ILogger Logger = Log.ForContext<TransactionCommandDecorator<TRequest, TResponse>>(); 

        public TransactionCommandDecorator
        (
            BillingContext dbContext,
            IBillingIntegrationEventService billingIntegrationEventService
        )
        {
            _dbContext = dbContext;
            _billingIntegrationEventService = billingIntegrationEventService;
        }

        public async Task<Result<TResponse>> Handle
        (
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<Result<TResponse>> next
        )
        {
            Result<TResponse> response = default;
            string requestTypeName = request.GetType().FullName;

            try
            {
                // Handling retries
                if (_dbContext.HasActiveTransaction) return await next();

                IExecutionStrategy strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;

                    await using (IDbContextTransaction transaction = await _dbContext.BeginTransactionAsync())
                    using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                    {
                        Logger.Information
                        (
                            "Begin transaction {TransactionId} for {CommandName} ({@Command})",
                            transaction.TransactionId, requestTypeName, request
                        );

                        response = await next();

                        Logger.Information
                        (
                            "Commit transaction {TransactionId} for {CommandName}",
                            transaction.TransactionId, requestTypeName
                        );

                        await _dbContext.CommitTransactionAsync(transaction, cancellationToken);

                        transactionId = transaction.TransactionId;
                    }

                    await _billingIntegrationEventService.PublishEventsAsync(transactionId, cancellationToken);
                });

                return response;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error Handling transaction for {CommandName} ({@Command})", requestTypeName, request);

                throw;
            }
        }
    }
}