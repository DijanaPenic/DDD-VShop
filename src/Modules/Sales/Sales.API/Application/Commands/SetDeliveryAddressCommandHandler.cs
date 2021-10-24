using OneOf;
using OneOf.Types;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventStore;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Application.Commands;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.Domain.Models.ShoppingCartAggregate;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class SetDeliveryAddressCommandHandler : ICommandHandler<SetDeliveryAddressCommand, Success>
    {
        private readonly IEventStoreAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;
        
        public SetDeliveryAddressCommandHandler(IEventStoreAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }
        
        public async Task<OneOf<Success, ApplicationError>> Handle(SetDeliveryAddressCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync(EntityId.Create(command.ShoppingCartId));
            if (shoppingCart is null) return NotFoundError.Create("Shopping cart not found.");
            
            Option<ApplicationError> errorResult = shoppingCart.Customer.SetDeliveryAddress
            (
                Address.Create
                (
                    command.City,
                    command.CountryCode,
                    command.PostalCode,
                    command.StateProvince,
                    command.StreetAddress
                )
            );
            
            if (errorResult.IsSome(out ApplicationError error)) return error;

            await _shoppingCartRepository.SaveAsync(shoppingCart);

            return new Success();
        }
    }
}