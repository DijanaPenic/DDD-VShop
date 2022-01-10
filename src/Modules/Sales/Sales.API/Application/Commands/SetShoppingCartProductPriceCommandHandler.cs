// using System;
// using System.Threading;
// using System.Threading.Tasks;
//
// using VShop.SharedKernel.Infrastructure;
// using VShop.SharedKernel.Messaging.Commands;
// using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
// using VShop.SharedKernel.Domain.ValueObjects;
// using VShop.SharedKernel.EventSourcing.Stores.Contracts;
// using VShop.Modules.Sales.Domain.Models.ShoppingCart;
//
// namespace VShop.Modules.Sales.API.Application.Commands
// {
//     public class SetShoppingCartProductPriceCommandHandler : ICommandHandler<SetShoppingCartProductPriceCommand>
//     {
//         private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;
//         
//         public SetShoppingCartProductPriceCommandHandler(IAggregateStore<ShoppingCart> shoppingCartStore)
//             => _shoppingCartStore = shoppingCartStore;
//         
//         public async Task<Result> Handle(SetShoppingCartProductPriceCommand command, CancellationToken cancellationToken)
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
//                 Result setProductPriceResult = shoppingCart.SetProductPrice
//                 (
//                     EntityId.Create(command.ProductId).Data,
//                     Price.Create(command.UnitPrice).Data
//                 );
//                 if (setProductPriceResult.IsError) return setProductPriceResult.Error;
//             }
//
//             await _shoppingCartStore.SaveAndPublishAsync(shoppingCart, cancellationToken);
//
//             return Result.Success;
//         }
//     }
//     
//     public record SetShoppingCartProductPriceCommand : Command
//     {
//         public Guid ShoppingCartId { get; init; }
//         public Guid ProductId { get; init; }
//         public decimal UnitPrice { get; init; }
//         
//         public SetShoppingCartProductPriceCommand() { }
//
//         public SetShoppingCartProductPriceCommand
//         (
//             Guid shoppingCartId,
//             Guid productId,
//             decimal unitPrice
//         )
//         {
//             ShoppingCartId = shoppingCartId;
//             ProductId = productId;
//             UnitPrice = unitPrice;
//         }
//     }
// }