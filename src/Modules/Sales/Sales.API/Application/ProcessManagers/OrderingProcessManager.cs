using System;
using System.Linq;
using Newtonsoft.Json;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.EventSourcing.ProcessManagers;

namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    internal class OrderingProcessManager : ProcessManager
    {
        public Guid ShoppingCartId { get; private set; }
        public OrderFulfillmentStatus Status { get; private set; }

        public OrderingProcessManager()
        {
            RegisterEvent<ShoppingCartCheckoutRequestedDomainEvent>(Handle);
            RegisterEvent<OrderPlacedDomainEvent>(Handle);
            RegisterCommand<ReminderCommand>(Handle);
        }
        
        public void Handle(ReminderCommand command)
        {
            
        }
        
        public void Handle(ShoppingCartCheckoutRequestedDomainEvent @event)
        {
            PlaceOrderCommand placeOrderCommand = new()
            {
                OrderId = @event.OrderId,
                DeliveryCost = @event.DeliveryCost,
                TotalDiscount = @event.TotalDiscount,
                CustomerId = @event.CustomerId,
                FirstName = @event.FirstName,
                MiddleName = @event.MiddleName,
                LastName = @event.LastName,
                EmailAddress = @event.EmailAddress,
                PhoneNumber = @event.PhoneNumber,
                City = @event.City,
                CountryCode = @event.CountryCode,
                PostalCode = @event.PostalCode,
                StateProvince = @event.StateProvince,
                StreetAddress = @event.StreetAddress,
                OrderItems = @event.Items.Select(sci => new OrderItemCommandDto
                {
                    ProductId = sci.ProductId,
                    Quantity = sci.Quantity,
                    UnitPrice = sci.UnitPrice
                }).ToArray()
            };
            RaiseCommand(placeOrderCommand);

            // TODO - test scheduling command + create Command > ReminderCommand converter
            ReminderCommand reminderCommand = new()
            {
                ProcessId = @event.OrderId,
                Status = (int)Status,
                Command = JsonConvert.SerializeObject(placeOrderCommand),
                Type = placeOrderCommand.GetType().Name
            };
            ScheduleCommand(reminderCommand, DateTime.UtcNow.AddSeconds(10));
        }

        public void Handle(OrderPlacedDomainEvent _)
        {
            DeleteShoppingCartCommand deleteShoppingCartCommand = new() { ShoppingCartId = ShoppingCartId };
            RaiseCommand(deleteShoppingCartCommand);
        }

        protected override void ApplyEvent(IBaseEvent @event)
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