using MediatR;
using FluentValidation;
using FluentValidation.Results;

using VShop.SharedKernel.Infrastructure.Queries.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.SharedKernel.Application.Decorators
{
    public class ValidationCommandDecorator<TCommand, TResponse> : ICommandDecorator<TCommand, TResponse>
        where TCommand : IRequest<TResponse>
        where TResponse : class
    {
        private readonly IEnumerable<IValidator<TCommand>> _validators;

        public ValidationCommandDecorator(IEnumerable<IValidator<TCommand>> validators) 
            => _validators = validators;
        
        public async Task<TResponse> Handle
        (
            TCommand command,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next
        )
        {
            // TODO - Temp. fix
            if (command is IBaseQuery) return await next();

            ValidationContext<TCommand> context = new(command);
            IList<ValidationFailure> failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count is not 0) throw new ValidationException(failures);

            return await next();
        }
    }
}