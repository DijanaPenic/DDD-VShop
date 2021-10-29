using System;

using VShop.SharedKernel.PostgresDb;
using VShop.Modules.Sales.Domain.ProcessManagers;

namespace VShop.Modules.Sales.Infrastructure.Entities
{
    public class OrderFulfillmentProcess : DbBaseEntity
    {
        public Guid OrderId { get; set; }
        public OrderFulfillmentProcessManager.OrderFulfillmentStatus Status { get; set; }
    }
}