using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.Domain.Models.Ordering;

namespace VShop.Modules.Sales.Domain.Services
{
    public interface IShoppingCartOrderingService
    {
        Task<Result<Order>> CreateOrderAsync
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Guid causationId,
            CancellationToken cancellationToken = default
        );
    }
}