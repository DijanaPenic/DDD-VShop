using Serilog;
using System.Threading;
using System.Threading.Tasks;

using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Infrastructure;

namespace VShop.Modules.Sales.Application.ProcessManagers
{
    public class OrderFulfillmentProcessManager
    {
        private readonly ILogger _logger;
        private readonly SalesContext _dbContext;
        
        public OrderFulfillmentProcessManager
        (
            ILogger logger,
            SalesContext dbContext
        )
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        
        public Task Handle(ShoppingCartCheckoutRequestedDomainEvent @event, CancellationToken cancellationToken)
        {
            _logger.Information("Handling domain event: {DomainEvent}", nameof(ShoppingCartCheckoutRequestedDomainEvent));
            
            return Task.CompletedTask;
        }
    }
}