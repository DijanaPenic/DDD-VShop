using System.Threading;
using System.Threading.Tasks;

using VShop.Modules.Billing.Infrastructure;
using VShop.Modules.Catalog.Integration.Events;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;

namespace VShop.Modules.Billing.API.Application.EventHandlers
{
    public class OrderStockConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>
    {
        private readonly BillingContext _billingContext;

        public OrderStockConfirmedIntegrationEventHandler
        (
            BillingContext billingContext
        )
        {
            _billingContext = billingContext;
        }

        public Task Handle(OrderStockConfirmedIntegrationEvent @event, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}