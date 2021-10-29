using Serilog;
using System.Threading;
using System.Threading.Tasks;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Infrastructure;
using VShop.Modules.Sales.Infrastructure.Entities;
using VShop.SharedKernel.Application.Events;
using VShop.SharedKernel.Application.Commands;

namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    public class OrderFulfillmentProcessManager :
        IDomainEventHandler<ShoppingCartCheckoutRequestedDomainEvent>
    {
        private readonly ILogger _logger;
        private readonly SalesContext _dbContext;
        private readonly ICommandBus _commandBus;
        
        public OrderFulfillmentProcessManager
        (
            ILogger logger,
            ICommandBus commandBus,
            SalesContext dbContext
        )
        {
            _logger = logger;
            _commandBus = commandBus;
            _dbContext = dbContext;
        }
        
        // TODO - placing an order is synchronous, but fulfilling your order is not
        public async Task Handle(ShoppingCartCheckoutRequestedDomainEvent @event, CancellationToken cancellationToken)
        {
            _logger.Information("Handling domain event: {DomainEvent}", nameof(ShoppingCartCheckoutRequestedDomainEvent));
            
            _dbContext.OrderFulfillmentProcesses.Add(new OrderFulfillmentProcess
            {
                OrderId = @event.ShoppingCartId, // TODO - need to handler OrderId mapping instead
                Status = OrderFulfillmentStatus.CheckoutRequested
            });
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}