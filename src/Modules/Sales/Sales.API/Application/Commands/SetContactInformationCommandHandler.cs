using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class SetContactInformationCommandHandler : ICommandHandler<SetContactInformationCommand>
    {
        private readonly IAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;

        public SetContactInformationCommandHandler(IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository)
            => _shoppingCartRepository = shoppingCartRepository;
        
        public async Task<Result> Handle(SetContactInformationCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId),
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");
            
            Result setContactInformationResult = shoppingCart.Customer.SetContactInformation
            (
                FullName.Create(command.FirstName, command.MiddleName, command.LastName),
                EmailAddress.Create(command.EmailAddress),
                PhoneNumber.Create(command.PhoneNumber),
                command.Gender
            );
            
            if (setContactInformationResult.IsError(out ApplicationError error)) return error;

            await _shoppingCartRepository.SaveAsync(shoppingCart, cancellationToken);

            return Result.Success;
        }
    }
    
    public record SetContactInformationCommand : Command
    {
        public Guid ShoppingCartId { get; set; }
        public string FirstName { get; init; }
        public string MiddleName { get; init; }
        public string LastName { get; init; }
        public string EmailAddress { get; init; }
        public string PhoneNumber { get; init; }
        public GenderType Gender { get; init; }
    }
}