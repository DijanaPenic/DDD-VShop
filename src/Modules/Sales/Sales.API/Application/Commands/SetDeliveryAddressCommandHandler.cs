using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class SetDeliveryAddressCommandHandler : ICommandHandler<SetDeliveryAddressCommand>
    {
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;
        
        public SetDeliveryAddressCommandHandler(IAggregateStore<ShoppingCart> shoppingCartStore)
        => _shoppingCartStore = shoppingCartStore;
        
        public async Task<Result> Handle(SetDeliveryAddressCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId).Value,
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");
            
            Result<Address> addressResult = Address.Create
            (
                command.City,
                command.CountryCode,
                command.PostalCode,
                command.StateProvince,
                command.StreetAddress
            );
            if (addressResult.IsError) return addressResult.Error;
            
            Result setDeliveryAddressResult = shoppingCart.Customer.SetDeliveryAddress(addressResult.Value);
            
            if (setDeliveryAddressResult.IsError) return setDeliveryAddressResult.Error;

            await _shoppingCartStore.SaveAndPublishAsync(shoppingCart, cancellationToken);

            return Result.Success;
        }
    }
    
    public record SetDeliveryAddressCommand : Command
    {
        public Guid ShoppingCartId { get; init; }
        public string City { get; init; }
        public string CountryCode { get; init; }
        public string PostalCode { get; init; }
        public string StateProvince { get; init; }
        public string StreetAddress { get; init; }
    }
}