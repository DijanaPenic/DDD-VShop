using FluentValidation;
using FluentValidation.Results;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.SharedKernel.Application.Decorators;

public sealed class ValidationCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>, IDecorator
    where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> _handler;
    private readonly IEnumerable<IValidator<TCommand>> _validators;

    public ValidationCommandHandlerDecorator
    (
        ICommandHandler<TCommand> handler,
        IEnumerable<IValidator<TCommand>> validators
    )
    {
        _handler = handler;
        _validators = validators;
    }

    public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        ValidationContext<TCommand> context = new(command);
        IList<ValidationFailure> failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count is not 0) throw new ValidationException(failures);

        return await _handler.HandleAsync(command, cancellationToken);
    }
}

public sealed class ValidationCommandHandlerDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>, IDecorator
    where TCommand : class, ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _handler;
    private readonly IEnumerable<IValidator<TCommand>> _validators;

    public ValidationCommandHandlerDecorator
    (
        ICommandHandler<TCommand, TResult> handler,
        IEnumerable<IValidator<TCommand>> validators
    )
    {
        _handler = handler;
        _validators = validators;
    }

    public async Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        ValidationContext<TCommand> context = new(command);
        IList<ValidationFailure> failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count is not 0) throw new ValidationException(failures);

        return await _handler.HandleAsync(command, cancellationToken);
    }
}
