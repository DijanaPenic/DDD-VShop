using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Infrastructure.Queries;
using VShop.Modules.Sales.Infrastructure.Commands.Shared;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Infrastructure.Queries.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Commands.Handlers
{
    internal class CreateShoppingCartCommandHandler : ICommandHandler<CreateShoppingCartCommand>
    {
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;
        private readonly IShoppingCartReadService _readService;

        public CreateShoppingCartCommandHandler
        (
            IAggregateStore<ShoppingCart> shoppingCartStore,
            IShoppingCartReadService readService
        )
        {
            _shoppingCartStore = shoppingCartStore;
            _readService = readService;
        }

        public async Task<Result> Handle
        (
            CreateShoppingCartCommand command,
            CancellationToken cancellationToken
        )
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId).Data, // TODO - improve validation in commands.
                command.Metadata.MessageId,
                cancellationToken
            );
            
            if (shoppingCart is not null) return Result.Success;
            
            bool hasShoppingCart = (await _readService.GetActiveShoppingCartByCustomerIdAsync(command.CustomerId)) is not null;
            if (hasShoppingCart)
                return Result.ValidationError("Only one active shopping cart is supported per customer.");

            Result<ShoppingCart> createShoppingCartResult = ShoppingCart.Create
            (
                EntityId.Create(command.ShoppingCartId).Data,
                EntityId.Create(command.CustomerId).Data,
                Discount.Create(command.CustomerDiscount).Data
            );
            if (createShoppingCartResult.IsError) return createShoppingCartResult.Error;

            shoppingCart = createShoppingCartResult.Data;
            foreach (ShoppingCartProductCommandDto shoppingCartItem in command.ShoppingCartItems)
            {
                Result addProductResult = shoppingCart.AddProductQuantity
                (
                    EntityId.Create(shoppingCartItem.ProductId).Data,
                    ProductQuantity.Create(shoppingCartItem.Quantity).Data,
                    Price.Create(shoppingCartItem.UnitPrice.DecimalValue).Data
                );
                if (addProductResult.IsError) return addProductResult.Error;
            }

            await _shoppingCartStore.SaveAndPublishAsync
            (
                shoppingCart,
                command.Metadata.MessageId,
                command.Metadata.CorrelationId,
                cancellationToken
            );

            return Result.Success;
        }
    }
}