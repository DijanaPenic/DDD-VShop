using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class SetContactInformationCommandHandler : ICommandHandler<SetContactInformationCommand>
    {
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;

        public SetContactInformationCommandHandler(IAggregateStore<ShoppingCart> shoppingCartStore)
            => _shoppingCartStore = shoppingCartStore;
        
        public async Task<Result> Handle(SetContactInformationCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId).Value,
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");

            if (shoppingCart.OutboxMessageCount is 0)
            {
                Result<FullName> fullNameResult = FullName.Create(command.FirstName, command.MiddleName, command.LastName);
                if (fullNameResult.IsError) return fullNameResult.Error;
            
                Result setContactInformationResult = shoppingCart.Customer.SetContactInformation
                (
                    fullNameResult.Value,
                    EmailAddress.Create(command.EmailAddress).Value,
                    PhoneNumber.Create(command.PhoneNumber).Value,
                    command.Gender
                );
                if (setContactInformationResult.IsError) return setContactInformationResult.Error;
            }

            await _shoppingCartStore.SaveAndPublishAsync(shoppingCart, cancellationToken);

            return Result.Success;
        }
    }
    
    public record SetContactInformationCommand : Command
    {
        public Guid ShoppingCartId { get; init; }
        public string FirstName { get; init; }
        public string MiddleName { get; init; }
        public string LastName { get; init; }
        public string EmailAddress { get; init; }
        public string PhoneNumber { get; init; }
        public GenderType Gender { get; init; }
    }
}