﻿using System;
using OneOf;
using OneOf.Types;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Application.Commands;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventStore.Repositories.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.Commands.Shared;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class AddShoppingCartProductCommandHandler : ICommandHandler<AddShoppingCartProductCommand, Success>
    {
        private readonly IEventStoreAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;
        
        public AddShoppingCartProductCommandHandler(IEventStoreAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }
        
        public async Task<OneOf<Success, ApplicationError>> Handle(AddShoppingCartProductCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync(EntityId.Create(command.ShoppingCartId));

            if (shoppingCart is null) return NotFoundError.Create("Shopping cart not found.");
            
            Option<ApplicationError> errorResult = shoppingCart.AddProduct
            (
                EntityId.Create(command.ShoppingCartItem.ProductId),
                ProductQuantity.Create(command.ShoppingCartItem.Quantity),
                Price.Create(command.ShoppingCartItem.UnitPrice)
            );
            
            if (errorResult.IsSome(out ApplicationError error)) return error;
        
            await _shoppingCartRepository.SaveAsync(shoppingCart);

            return new Success();
        }
    }
    
    public record AddShoppingCartProductCommand : ICommand<Success>
    {
        public Guid ShoppingCartId { get; set; }
        public ShoppingCartItemDto ShoppingCartItem { get; set; }
    }
}