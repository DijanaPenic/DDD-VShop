using OneOf;
using OneOf.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Messaging.Commands;
using VShop.SharedKernel.Infrastructure.Messaging.Commands.Publishing;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class SetDeliveryAddressCommandHandler : ICommandHandler<SetDeliveryAddressCommand, Success>
    {
        private readonly IAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;
        
        public SetDeliveryAddressCommandHandler(IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }
        
        public async Task<OneOf<Success, ApplicationError>> Handle(SetDeliveryAddressCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId),
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
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

            await _shoppingCartRepository.SaveAsync(shoppingCart, cancellationToken);

            return new Success();
        }
    }
    
    public record SetDeliveryAddressCommand : BaseCommand<Success>
    {
        public Guid ShoppingCartId { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string PostalCode { get; set; }
        public string StateProvince { get; set; }
        public string StreetAddress { get; set; }
    }
}