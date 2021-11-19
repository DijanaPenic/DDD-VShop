using OneOf;
using OneOf.Types;
using System;
using System.Threading.Tasks;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.Modules.Sales.Domain.Services
{
    public interface IShoppingCartOrderingService
    {
        Task<OneOf<Success<Order>, ApplicationError>> CreateOrderAsync(EntityId shoppingCartId, EntityId orderId, Guid messageId, Guid correlationId);
    }
}