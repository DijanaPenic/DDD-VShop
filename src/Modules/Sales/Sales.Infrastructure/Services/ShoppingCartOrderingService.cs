using OneOf;
using OneOf.Types;
using System;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.Infrastructure.Services
{
    public class ShoppingCartOrderingService : IShoppingCartOrderingService
    {
        private readonly IAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;

        public ShoppingCartOrderingService(IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository)
            => _shoppingCartRepository = shoppingCartRepository;

        public async Task<OneOf<Success<Order>, ApplicationError>> CreateOrderAsync
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Guid messageId,
            Guid correlationId
        )
        {
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync(shoppingCartId);
            
            Order order = Order.Create
            (
                orderId,
                shoppingCart.DeliveryCost,
                shoppingCart.TotalDiscount,
                shoppingCart.Customer.CustomerId,
                shoppingCart.Customer.FullName,
                shoppingCart.Customer.EmailAddress,
                shoppingCart.Customer.PhoneNumber,
                shoppingCart.Customer.DeliveryAddress,
                messageId,
                correlationId
            );
            
            foreach (ShoppingCartItem item in shoppingCart.Items)
            {
                Option<ApplicationError> errorResult = order.AddOrderItem
                (
                    item.Id,
                    item.Quantity,
                    item.UnitPrice
                );

                if (errorResult.IsSome(out ApplicationError error)) return error;
            }

            return new Success<Order>(order);
        }
    }
}