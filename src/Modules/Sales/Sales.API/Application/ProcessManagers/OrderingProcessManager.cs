using System;
using System.Linq;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.EventSourcing.ProcessManagers;

namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    public class OrderingProcessManager : ProcessManager
    {
        public Guid ShoppingCartId { get; private set; }
        public OrderFulfillmentStatus Status { get; private set; }

        public void Transition(ShoppingCartCheckoutRequestedDomainEvent @event, ShoppingCart shoppingCart)
        {
            Apply(@event);
            
            PlaceOrderCommand placeOrderCommand = new()
            {
                OrderId = @event.OrderId,
                DeliveryCost = shoppingCart.DeliveryCost,
                TotalDiscount = shoppingCart.TotalDiscount,
                CustomerId = shoppingCart.Customer.CustomerId,
                FirstName = shoppingCart.Customer.FullName.FirstName,
                MiddleName = shoppingCart.Customer.FullName.MiddleName,
                LastName = shoppingCart.Customer.FullName.LastName,
                EmailAddress = shoppingCart.Customer.EmailAddress,
                PhoneNumber = shoppingCart.Customer.PhoneNumber,
                City = shoppingCart.Customer.DeliveryAddress.City,
                CountryCode = shoppingCart.Customer.DeliveryAddress.CountryCode,
                PostalCode = shoppingCart.Customer.DeliveryAddress.PostalCode,
                StateProvince = shoppingCart.Customer.DeliveryAddress.StateProvince,
                StreetAddress = shoppingCart.Customer.DeliveryAddress.StreetAddress,
                OrderItems = shoppingCart.Items.Select(sci => new OrderItemDto
                {
                    ProductId = sci.Id,
                    Quantity = sci.Quantity,
                    UnitPrice = sci.UnitPrice
                }).ToArray(),
                CausationId = @event.MessageId,
                CorrelationId = @event.CorrelationId
            };
            EnqueueCommands(placeOrderCommand);
        }

        public void Transition(OrderPlacedDomainEvent @event)
        {
            Apply(@event);
            
            DeleteShoppingCartCommand deleteShoppingCartCommand = new() { ShoppingCartId = Id };
            EnqueueCommands(deleteShoppingCartCommand);
        }

        protected override void When(IMessage @event)
        {
            switch (@event)
            {
                case ShoppingCartCheckoutRequestedDomainEvent e:
                    Id = e.OrderId;
                    ShoppingCartId = e.ShoppingCartId;
                    Status = OrderFulfillmentStatus.CheckoutRequested;
                    break;
                case OrderPlacedDomainEvent _:
                    Status = OrderFulfillmentStatus.OrderPlaced;
                    break;
            }
        }
    }
}