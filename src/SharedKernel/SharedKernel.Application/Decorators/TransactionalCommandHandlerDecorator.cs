using Serilog;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.PostgresDb.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;

namespace VShop.SharedKernel.Application.Decorators;

public sealed class TransactionalCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>, IDecorator
    where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> _handler;
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIntegrationEventService _integrationEventService;

    public TransactionalCommandHandlerDecorator
    (
        ICommandHandler<TCommand> handler,
        ILogger logger,
        IUnitOfWork unitOfWork,
        IIntegrationEventService integrationEventService
    )
    {
        _handler = handler;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _integrationEventService = integrationEventService;
    }

    public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        string commandTypeName = command.GetType().Name;
        
        (Guid transactionId, Result response) = await _unitOfWork.ExecuteAsync(() 
            => _handler.HandleAsync(command, cancellationToken), cancellationToken);
            
        await _integrationEventService.PublishEventsAsync(transactionId, cancellationToken);
            
        _logger.Information
        (
            "Commit transaction {TransactionId} for {CommandName}",
            transactionId, commandTypeName
        );

        return response;
    }
}

public sealed class TransactionalCommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>, IDecorator
    where TCommand : class, ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _handler;
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIntegrationEventService _integrationEventService;

    public TransactionalCommandHandlerDecorator
    (
        ICommandHandler<TCommand, TResult> handler,
        ILogger logger,
        IUnitOfWork unitOfWork,
        IIntegrationEventService integrationEventService
    )
    {
        _handler = handler;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _integrationEventService = integrationEventService;
    }

    public async Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        string commandTypeName = command.GetType().Name;
        
        (Guid transactionId, Result<TResult> response) = await _unitOfWork.ExecuteAsync(() 
            => _handler.HandleAsync(command, cancellationToken), cancellationToken);
            
        await _integrationEventService.PublishEventsAsync(transactionId, cancellationToken);
            
        _logger.Information
        (
            "Commit transaction {TransactionId} for {CommandName}",
            transactionId, commandTypeName
        );

        return response;
    }
}