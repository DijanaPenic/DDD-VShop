using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.Domain.Models.Ordering;

[assembly: InternalsVisibleTo("VShop.Modules.Sales.API")]
namespace VShop.Modules.Sales.Domain.Services
{
    internal interface IShoppingCartOrderingService
    {
        Task<Result<Order>> CreateOrderAsync
        (
            EntityId shoppingCartId,
            Guid causationId,
            CancellationToken cancellationToken = default
        );
    }
}