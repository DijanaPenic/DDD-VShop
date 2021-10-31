using System;

using VShop.SharedKernel.PostgresDb;
using VShop.Modules.Sales.Domain.Enums;

namespace VShop.Modules.Sales.Infrastructure.Entities
{
    public class OrderFulfillmentProcess : DbBaseEntity
    {
        public Guid OrderId { get; set; }
        public Guid ShoppingCartId { get; set; }
        public Guid CustomerId { get; set; }
        public OrderFulfillmentStatus Status { get; set; }
    }
}