using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class SetDeliveryAddressCommandHandler : ICommandHandler<SetDeliveryAddressCommand>
    {
        private readonly IAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;
        
        public SetDeliveryAddressCommandHandler(IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository)
        => _shoppingCartRepository = shoppingCartRepository;
        
        public async Task<Result> Handle(SetDeliveryAddressCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId),
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");
            
            Result setDeliveryAddressResult = shoppingCart.Customer.SetDeliveryAddress
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
            
            if (setDeliveryAddressResult.IsError(out ApplicationError error)) return error;

            await _shoppingCartRepository.SaveAsync(shoppingCart, cancellationToken);

            return Result.Success;
        }
    }
    
    public record SetDeliveryAddressCommand : Command
    {
        public Guid ShoppingCartId { get; set; }
        public string City { get; init; }
        public string CountryCode { get; init; }
        public string PostalCode { get; init; }
        public string StateProvince { get; init; }
        public string StreetAddress { get; init; }
    }
}