using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.Domain.Models.Ordering;

namespace VShop.Modules.Sales.Domain.Services
{
    internal interface IShoppingCartOrderingService
    {
        Task<Result<Order>> CreateOrderAsync
        (
            EntityId shoppingCartId,
            CancellationToken cancellationToken = default
        );
    }
}