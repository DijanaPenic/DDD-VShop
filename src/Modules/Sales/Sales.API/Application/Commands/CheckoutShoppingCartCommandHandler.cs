using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class CheckoutShoppingCartCommandHandler : ICommandHandler<CheckoutShoppingCartCommand, CheckoutOrder>
    {
        private readonly IClockService _clockService;
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;

        public CheckoutShoppingCartCommandHandler(IClockService clockService, IAggregateStore<ShoppingCart> shoppingCartStore)
        {
            _clockService = clockService;
            _shoppingCartStore = shoppingCartStore;
        }

        public async Task<Result<CheckoutOrder>> Handle(CheckoutShoppingCartCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId).Value,
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");

            Guid orderId;

            if (shoppingCart.OutboxMessageCount is 0)
            {
                orderId = SequentialGuid.Create();
                
                Result checkoutResult = shoppingCart.RequestCheckout(EntityId.Create(orderId).Value, _clockService.Now);
                if (checkoutResult.IsError) return checkoutResult.Error;
            }
            else
            {
                ShoppingCartCheckoutRequestedDomainEvent checkoutDomainEvent = shoppingCart
                    .GetOutboxMessages<ShoppingCartCheckoutRequestedDomainEvent>()
                    .SingleOrDefault();
                if (checkoutDomainEvent is null) return Result.NotFoundError("ShoppingCartCheckoutRequestedDomainEvent not found.");
                
                orderId = checkoutDomainEvent.OrderId;
            }
            
            await _shoppingCartStore.SaveAndPublishAsync(shoppingCart, cancellationToken);

            return new CheckoutOrder(orderId);
        }
    }

    public record CheckoutShoppingCartCommand : Command<CheckoutOrder>
    {
        public Guid ShoppingCartId { get; }
        
        public CheckoutShoppingCartCommand(Guid shoppingCartId)
        {
            ShoppingCartId = shoppingCartId;
        }
    }
    
    public record CheckoutOrder
    {
        public Guid OrderId { get; }
        
        public CheckoutOrder(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}