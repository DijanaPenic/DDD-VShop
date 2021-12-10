﻿using System;
using NodaTime;
using Newtonsoft.Json;

using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Billing.Integration.Events;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    internal class OrderingProcessManager : ProcessManager
    {
        public Guid ShoppingCartId { get; private set; }
        public OrderingProcessManagerStatus Status { get; private set; }
        public int ShippingCheckCount { get; private set; }

        public OrderingProcessManager()
        {
            RegisterEvent<ShoppingCartCheckoutRequestedDomainEvent>(Handle);
            RegisterEvent<OrderPlacedDomainEvent>(Handle);
            RegisterEvent<PaymentSucceededIntegrationEvent>(Handle);
            RegisterEvent<PaymentFailedIntegrationEvent>(Handle);
            RegisterEvent<ShippingGracePeriodExpiredDomainEvent>(Handle);
            RegisterEvent<PaymentGracePeriodExpiredDomainEvent>(Handle);
        }

        public void Handle(ShoppingCartCheckoutRequestedDomainEvent @event, IClockService clockService)
        {
            PlaceOrderCommand placeOrderCommand = new()
            {
                OrderId = Id,
                ShoppingCartId = ShoppingCartId
            };
            RaiseCommand(placeOrderCommand);
        }

        public void Handle(OrderPlacedDomainEvent @event, IClockService clockService)
        {
            DeleteShoppingCartCommand deleteShoppingCartCommand = new(ShoppingCartId);
            RaiseCommand(deleteShoppingCartCommand);
        }
        
        public void Handle(PaymentSucceededIntegrationEvent @event, IClockService clockService)
            => ScheduleDomainEvent
            (
                new ShippingGracePeriodExpiredDomainEvent(Id, JsonConvert.SerializeObject(@event)),
                clockService.Now.Plus(Duration.FromHours(Settings.ShippingGracePeriodInHours))
            );
        
        public void Handle(ShippingGracePeriodExpiredDomainEvent @event, IClockService clockService)
        {
            // The order is already shipped so there is nothing to address.
            if (Status is OrderingProcessManagerStatus.OrderShipped) return;

            // Cancelling the order as we already escalated the order request two times. Better alternative would be
            // to send out an email to the support team as we need to report a problem with the shipping department.
            if (ShippingCheckCount >= Settings.ShippingCheckThreshold)
            {
                CancelOrderCommand cancelOrderCommand = new(Id);
                RaiseCommand(cancelOrderCommand);
            }
            // Resend the payment status message to the shipping department (kind of a reminder message).
            else
            {
                RaiseIntegrationEvent(JsonConvert.DeserializeObject<PaymentSucceededIntegrationEvent>(@event.Content));
            }
        }

        public void Handle(PaymentFailedIntegrationEvent @event, IClockService clockService)
            => ScheduleDomainEvent
            (
                new PaymentGracePeriodExpiredDomainEvent(Id),
                clockService.Now.Plus(Duration.FromMinutes(Settings.PaymentGracePeriodInMinutes))
            );
        
        public void Handle(PaymentGracePeriodExpiredDomainEvent @event, IClockService clockService)
        {
            // User managed to successfully pay the order.
            if (Status is not OrderingProcessManagerStatus.OrderPaymentFailed) return;

            // User didn't manage to pay so we need to cancel the order.
            CancelOrderCommand cancelOrderCommand = new(Id);
            RaiseCommand(cancelOrderCommand);
        }

        protected override void ApplyEvent(IBaseEvent @event)
        {
            switch (@event)
            {
                case ShoppingCartCheckoutRequestedDomainEvent e:
                    Id = e.OrderId;
                    ShoppingCartId = e.ShoppingCartId;
                    Status = OrderingProcessManagerStatus.CheckoutRequested;
                    break;
                case OrderPlacedDomainEvent _:
                    Status = OrderingProcessManagerStatus.OrderPlaced;
                    break;
                case PaymentSucceededIntegrationEvent _:
                    Status = OrderingProcessManagerStatus.OrderPaymentSucceeded;
                    break;
                case PaymentFailedIntegrationEvent _:
                    Status = OrderingProcessManagerStatus.OrderPaymentFailed;
                    break;
                case OrderStatusSetToCancelledDomainEvent _:
                    Status = OrderingProcessManagerStatus.OrderCancelled;
                    break;
                case ShippingGracePeriodExpiredDomainEvent _:
                    ShippingCheckCount++;
                    break;
            }
        }
        
        public static class Settings
        {
            public const int ShippingCheckThreshold = 3;
            public const int PaymentGracePeriodInMinutes = 30;
            public const int ShippingGracePeriodInHours = 24;
        }
    }
}