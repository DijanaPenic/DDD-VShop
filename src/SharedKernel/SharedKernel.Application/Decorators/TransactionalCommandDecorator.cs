using MediatR;
using Serilog;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.PostgresDb.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;

namespace VShop.SharedKernel.Application.Decorators
{
    public class TransactionalCommandDecorator<TCommand, TResponse> : ICommandDecorator<TCommand, TResponse>
        where TCommand : IRequest<TResponse>
        where TResponse : IResult
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventService _integrationEventService;

        public TransactionalCommandDecorator
        (
            ILogger logger,
            IUnitOfWork unitOfWork,
            IIntegrationEventService integrationEventService
        )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _integrationEventService = integrationEventService;
        }

        public async Task<TResponse> Handle
        (
            TCommand command,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next
        )
        {
            string commandTypeName = command.GetType().Name;

            try
            {
                (Guid transactionId, TResponse response) = await _unitOfWork.ExecuteAsync(() => next(), cancellationToken);
                await _integrationEventService.PublishEventsAsync(transactionId, cancellationToken);
                
                _logger.Information
                (
                    "Commit transaction {TransactionId} for {CommandName}",
                    transactionId, commandTypeName
                );

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