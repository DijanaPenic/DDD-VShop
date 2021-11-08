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
using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Sales.API.Application.Commands.Shared;
using VShop.Modules.Sales.Domain.Models.Ordering;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class PlaceOrderCommandHandler : ICommandHandler<PlaceOrderCommand, Success<Order>>
    {
        private readonly IAggregateRepository<Order, EntityId> _orderRepository;
        
        public PlaceOrderCommandHandler(IAggregateRepository<Order, EntityId> orderRepository)
        {
            _orderRepository = orderRepository;
        }
        
        public async Task<OneOf<Success<Order>, ApplicationError>> Handle(PlaceOrderCommand command, CancellationToken cancellationToken)
        {
            Order order = Order.Create
            (
                EntityId.Create(command.OrderId),
                Price.Create(command.DeliveryCost),
                Price.Create(command.TotalDiscount),
                EntityId.Create(command.CustomerId),
                FullName.Create
                (
                    command.FirstName,
                    command.MiddleName,
                    command.LastName
                ),
                EmailAddress.Create(command.EmailAddress),
                PhoneNumber.Create(command.PhoneNumber),
                Address.Create
                (
                    command.City,
                    command.CountryCode,
                    command.PostalCode,
                    command.StateProvince,
                    command.StreetAddress
                ),
                command.MessageId,
                command.CorrelationId
            );

            foreach (OrderItemDto orderItem in command.OrderItems)
            {
                Option<ApplicationError> errorResult = order.AddOrderItem
                (
                    EntityId.Create(orderItem.ProductId),
                    ProductQuantity.Create(orderItem.Quantity),
                    Price.Create(orderItem.UnitPrice)
                );

                if (errorResult.IsSome(out ApplicationError error)) return error;
            }
            
            order.RaiseEvent(new OrderPlacedIntegrationEvent{ OrderId = order.Id });
            
            await _orderRepository.SaveAsync(order);

            return new Success<Order>(order);
        }
    }
    
    public record PlaceOrderCommand : BaseCommand<Success<Order>>
    {
        public Guid OrderId { get; init; }
        public decimal DeliveryCost { get; init; }
        public decimal TotalDiscount { get; init; }
        public Guid CustomerId { get; init; }
        public string FirstName { get; init; }
        public string MiddleName { get; init; }
        public string LastName { get; init; }
        public string EmailAddress { get; init; }
        public string PhoneNumber { get; init; }
        public string City { get; init; }
        public string CountryCode { get; init; }
        public string PostalCode { get; init; }
        public string StateProvince { get; init; }
        public string StreetAddress { get; init; }
        public OrderItemDto[] OrderItems { get; init; }
    }
}