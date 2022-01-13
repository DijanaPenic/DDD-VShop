// using System;
// using System.Threading;
// using System.Threading.Tasks;
//
// using VShop.SharedKernel.Infrastructure;
// using VShop.SharedKernel.Infrastructure.Helpers;
// using VShop.SharedKernel.Infrastructure.Services.Contracts;
// using VShop.SharedKernel.Messaging.Commands;
// using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
// using VShop.SharedKernel.Domain.ValueObjects;
// using VShop.SharedKernel.EventSourcing.Stores.Contracts;
// using VShop.Modules.Sales.Domain.Models.ShoppingCart;
//
// namespace VShop.Modules.Sales.API.Application.Commands
// {
//     public class CheckoutShoppingCartCommandHandler : ICommandHandler<CheckoutShoppingCartCommand, CheckoutResponse>
//     {
//         private readonly IClockService _clockService;
//         private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;
//
//         public CheckoutShoppingCartCommandHandler(IClockService clockService, IAggregateStore<ShoppingCart> shoppingCartStore)
//         {
//             _clockService = clockService;
//             _shoppingCartStore = shoppingCartStore;
//         }
//
//         public async Task<Result<CheckoutResponse>> Handle(CheckoutShoppingCartCommand command, CancellationToken cancellationToken)
//         {
//             ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
//             (
//                 EntityId.Create(command.ShoppingCartId).Data,
//                 command.MessageId,
//                 command.CorrelationId,
//                 cancellationToken
//             );
//             if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");
//
//             if (shoppingCart.Events.Count is 0)
//             {
//                 Result checkoutResult = shoppingCart.Checkout
//                 (
//                     EntityId.Create(SequentialGuid.Create()).Data,
//                     _clockService.Now
//                 );
//                 if (checkoutResult.IsError) return checkoutResult.Error;
//             }
//
//             await _shoppingCartStore.SaveAndPublishAsync(shoppingCart, cancellationToken);
//
//             return new CheckoutResponse(shoppingCart.OrderId);
//         }
//     }
//
//     public record CheckoutResponse
//     {
//         public Guid OrderId { get; }
//         
//         public CheckoutResponse(Guid orderId)
//         {
//             OrderId = orderId;
//         }
//     }
// }