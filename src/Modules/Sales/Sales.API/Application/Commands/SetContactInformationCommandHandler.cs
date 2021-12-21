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
                command.ShoppingCartId,
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");
            
            Result setContactInformationResult = shoppingCart.Customer.SetContactInformation
            (
                command.FullName,
                command.EmailAddress,
                command.PhoneNumber,
                command.Gender
            );
            
            if (setContactInformationResult.IsError) return setContactInformationResult.Error;

            await _shoppingCartStore.SaveAndPublishAsync(shoppingCart, cancellationToken);

            return Result.Success;
        }
    }
    
    public record SetContactInformationCommand : Command
    {
        public EntityId ShoppingCartId { get; init; }
        public FullName FullName { get; init; }
        public EmailAddress EmailAddress { get; init; }
        public PhoneNumber PhoneNumber { get; init; }
        public GenderType Gender { get; init; }
    }
}