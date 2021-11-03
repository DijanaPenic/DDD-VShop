using OneOf;
using OneOf.Types;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Microsoft.EntityFrameworkCore;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.Infrastructure.Entities;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;
using VShop.SharedKernel.Application.Events;
using VShop.SharedKernel.Application.Commands;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.EventSourcing.Contracts;

namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    // TODO - implement base process manager class
    public class OrderFulfillmentProcessManager :
        IDomainEventHandler<ShoppingCartCheckoutRequestedDomainEvent>,
        IDomainEventHandler<OrderPlacedDomainEvent>
    {
        private readonly ILogger _logger;
        private readonly SalesContext _dbContext;
        private readonly ICommandBus _commandBus;
        private readonly IAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;
        
        public OrderFulfillmentProcessManager
        (
            ILogger logger,
            ICommandBus commandBus,
            SalesContext dbContext,
            IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository
        )
        {
            _logger = logger;
            _commandBus = commandBus;
            _dbContext = dbContext;
            _shoppingCartRepository = shoppingCartRepository;
        }
        
        public async Task Handle(ShoppingCartCheckoutRequestedDomainEvent @event, CancellationToken cancellationToken)
        {
            _logger.Information("Handling domain event: {DomainEvent}", nameof(ShoppingCartCheckoutRequestedDomainEvent));

            // TODO - idempo.
            Guid orderId = SequentialGuid.Create();
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync(EntityId.Create(@event.ShoppingCartId));
            
            // TODO - idempo.
            // Create a new OrderFulfillment process in the database
            _dbContext.OrderFulfillmentProcesses.Add(new OrderFulfillmentProcess
            {
                OrderId = orderId,
                ShoppingCartId = @event.ShoppingCartId,
                Status = OrderFulfillmentStatus.CheckoutRequested
            });
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Issue a "place order" command
            PlaceOrderCommand placeOrderCommand = new()
            {
                OrderId = orderId,
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
                    ProductId = sci.ProductId,
                    Quantity = sci.Quantity,
                    UnitPrice = sci.UnitPrice
                }).ToArray()
            };

            // TODO - idempo. - won't be a problem if I start using idempo. EntityId
            OneOf<Success<Order>, ApplicationError> placeOrderResult = await _commandBus.SendAsync(placeOrderCommand);
            if (placeOrderResult.IsT1) await TerminateProcessAsync(orderId, placeOrderResult.AsT1.ToString(), cancellationToken);
        }

        public async Task Handle(OrderPlacedDomainEvent @event, CancellationToken cancellationToken)
        {
            OrderFulfillmentProcess process = await GetProcessByOrderIdAsync(@event.OrderId, cancellationToken);
            process.Status = OrderFulfillmentStatus.OrderPlaced;
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            DeleteShoppingCartCommand deleteShoppingCartCommand = new() { ShoppingCartId = process.ShoppingCartId };
            
            OneOf<Success, ApplicationError> deleteShoppingCartResult = await _commandBus.SendAsync(deleteShoppingCartCommand);
            if (deleteShoppingCartResult.IsT1) await TerminateProcessAsync(@event.OrderId, deleteShoppingCartResult.AsT1.ToString(), cancellationToken);
        }
        
        private Task<OrderFulfillmentProcess> GetProcessByOrderIdAsync(Guid orderId, CancellationToken cancellationToken) 
            => _dbContext.OrderFulfillmentProcesses
                .FirstOrDefaultAsync(ofp => ofp.OrderId == orderId, cancellationToken);

        private async Task TerminateProcessAsync(Guid orderId, string reason, CancellationToken cancellationToken)
        {
            OrderFulfillmentProcess process = await _dbContext.OrderFulfillmentProcesses
                .SingleOrDefaultAsync(ofp => ofp.OrderId == orderId, cancellationToken);

            process.Status = OrderFulfillmentStatus.Terminated;
            process.Description = reason;
            
            _dbContext.OrderFulfillmentProcesses.Update(process);
            
            await _dbContext.SaveChangesAsync(cancellationToken);

            throw new Exception(reason);
        }
    }
}