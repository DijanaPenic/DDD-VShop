using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.SharedKernel.EventStore.Repositories;
using VShop.SharedKernel.Infrastructure.Messaging;

using static VShop.SharedKernel.Infrastructure.Messaging.MessageTypeMapper;

namespace VShop.Modules.Sales.API.Application
{
    public static class MessageMappings
    {
        public static void MapMessageTypes()
        {
            // Configure domain events
            AddCustomMap<ShoppingCartCreatedDomainEvent>(nameof(ShoppingCartCreatedDomainEvent));
            AddCustomMap<ShoppingCartCheckoutRequestedDomainEvent>(nameof(ShoppingCartCheckoutRequestedDomainEvent));
            AddCustomMap<ShoppingCartDeletionRequestedDomainEvent>(nameof(ShoppingCartDeletionRequestedDomainEvent));
            AddCustomMap<ShoppingCartItemQuantityDecreasedDomainEvent>(nameof(ShoppingCartItemQuantityDecreasedDomainEvent));
            AddCustomMap<ShoppingCartItemQuantityIncreasedDomainEvent>(nameof(ShoppingCartItemQuantityIncreasedDomainEvent));
            AddCustomMap<ShoppingCartContactInformationSetDomainEvent>(nameof(ShoppingCartContactInformationSetDomainEvent));
            AddCustomMap<ShoppingCartDeliveryAddressSetDomainEvent>(nameof(ShoppingCartDeliveryAddressSetDomainEvent));
            AddCustomMap<ShoppingCartDeliveryCostChangedDomainEvent>(nameof(ShoppingCartDeliveryCostChangedDomainEvent));
            AddCustomMap<ShoppingCartProductAddedDomainEvent>(nameof(ShoppingCartProductAddedDomainEvent));
            AddCustomMap<ShoppingCartProductRemovedDomainEvent>(nameof(ShoppingCartProductRemovedDomainEvent));
            AddCustomMap<OrderStatusSetToCancelledDomainEvent>(nameof(OrderStatusSetToCancelledDomainEvent));
            AddCustomMap<OrderStatusSetToShippedDomainEvent>(nameof(OrderStatusSetToShippedDomainEvent));
            AddCustomMap<OrderPlacedDomainEvent>(nameof(OrderPlacedDomainEvent));

            // Configure integration events
            AddCustomMap<OrderPlacedIntegrationEvent>(nameof(OrderPlacedIntegrationEvent));
            
            // Configure commands
            AddCustomMap<PlaceOrderCommand>(nameof(PlaceOrderCommand));
            AddCustomMap<DeleteShoppingCartCommand>(nameof(DeleteShoppingCartCommand));

            // Configure checkpoint
            AddCustomMap<Checkpoint>(nameof(Checkpoint));
            
            // Configure scheduled message
            AddCustomMap<ScheduledMessage>(nameof(ScheduledMessage));
        }
    }
}