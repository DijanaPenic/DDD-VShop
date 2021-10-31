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
using VShop.SharedKernel.EventStore.Repositories.Contracts;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Helpers;

namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    // TODO - implement base process manager class
    public class OrderFulfillmentProcessManager :
        IDomainEventHandler<ShoppingCartCheckoutRequestedDomainEvent>
    {
        private readonly ILogger _logger;
        private readonly SalesContext _dbContext;
        private readonly ICommandBus _commandBus;
        private readonly IEventStoreAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;
        
        public OrderFulfillmentProcessManager
        (
            ILogger logger,
            ICommandBus commandBus,
            SalesContext dbContext,
            IEventStoreAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository
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

            Guid orderId = GuidHelper.NewSequentialGuid();
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync(EntityId.Create(@event.ShoppingCartId));
            
            // Create a new OrderFulfillment process in the database
            _dbContext.OrderFulfillmentProcesses.Add(new OrderFulfillmentProcess
            {
                OrderId = orderId,
                // ShoppingCartId, CustomerId mapping
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

            OneOf<Success<Order>, ApplicationError> placeOrderResult = await _commandBus.Send(placeOrderCommand);
            if (placeOrderResult.IsT1)
            {
                await TerminateProcessAsync(orderId, placeOrderResult.AsT1.ToString(), cancellationToken);
            }
        }

        private async Task TerminateProcessAsync(Guid orderId, string reason, CancellationToken cancellationToken)
        {
            OrderFulfillmentProcess process = await _dbContext.OrderFulfillmentProcesses
                .SingleOrDefaultAsync(ofp => ofp.OrderId == orderId, cancellationToken);

            process.Status = OrderFulfillmentStatus.Terminated;
            // TODO - add information about the termination reason
            
            _dbContext.OrderFulfillmentProcesses.Update(process);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}