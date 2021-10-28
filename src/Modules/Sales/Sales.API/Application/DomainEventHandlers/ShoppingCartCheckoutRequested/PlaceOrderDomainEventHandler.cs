using Serilog;
using System.Threading;
using System.Threading.Tasks;

using VShop.Modules.Sales.Domain.Events;
using VShop.SharedKernel.Application.Events;

namespace VShop.Modules.Sales.API.Application.DomainEventHandlers
{
    public class PlaceOrderDomainEventHandler : IDomainEventHandler<ShoppingCartCheckoutRequestedDomainEvent>
    {
        private readonly ILogger _logger;

        public PlaceOrderDomainEventHandler
        (
            ILogger logger
        )
        {
            _logger = logger;
        }
        
        public Task Handle(ShoppingCartCheckoutRequestedDomainEvent @event, CancellationToken cancellationToken)
        {
            _logger.Information("Handling domain event: {DomainEvent}", nameof(ShoppingCartCheckoutRequestedDomainEvent));
            
            return Task.CompletedTask;
        }
    }
}