using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class SetDeliveryAddressCommandHandler : ICommandHandler<SetDeliveryAddressCommand>
    {
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;
        
        public SetDeliveryAddressCommandHandler(IAggregateStore<ShoppingCart> shoppingCartStore)
        => _shoppingCartStore = shoppingCartStore;
        
        public async Task<Result> Handle
        (
            SetDeliveryAddressCommand command,
            CancellationToken cancellationToken
        )
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId).Data,
                command.Metadata.MessageId,
                cancellationToken
            );
            
            if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");
            if (shoppingCart.IsRestored) return Result.Success;

            Result<Address> addressResult = Address.Create
            (
                command.City,
                command.CountryCode,
                command.PostalCode,
                command.StateProvince,
                command.StreetAddress
            );
            if (addressResult.IsError) return addressResult.Error;
            
            Result setDeliveryAddressResult = shoppingCart.Customer.SetDeliveryAddress(addressResult.Data);
            if (setDeliveryAddressResult.IsError) return setDeliveryAddressResult.Error;

            await _shoppingCartStore.SaveAndPublishAsync
            (
                shoppingCart,
                command.Metadata.MessageId,
                command.Metadata.CorrelationId,
                cancellationToken
            );
            
            return Result.Success;
        }
    }
}