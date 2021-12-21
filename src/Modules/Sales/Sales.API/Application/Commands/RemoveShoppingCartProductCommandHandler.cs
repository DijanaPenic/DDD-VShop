﻿using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class RemoveShoppingCartProductCommandHandler : ICommandHandler<RemoveShoppingCartProductCommand>
    {
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;
        
        public RemoveShoppingCartProductCommandHandler(IAggregateStore<ShoppingCart> shoppingCartStore)
            => _shoppingCartStore = shoppingCartStore;
        
        public async Task<Result> Handle(RemoveShoppingCartProductCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId).Value,
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");
            
            Result removeProductResult = shoppingCart.RemoveProduct
            (
                EntityId.Create(command.ProductId).Value,
                ProductQuantity.Create(command.Quantity).Value
            );
            
            if (removeProductResult.IsError) return removeProductResult.Error;

            await _shoppingCartStore.SaveAndPublishAsync(shoppingCart, cancellationToken);

            return Result.Success;
        }
    }
    
    public record RemoveShoppingCartProductCommand : Command
    {
        public Guid ShoppingCartId { get; init; }
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}