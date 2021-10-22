﻿using OneOf;
using OneOf.Types;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.EventStore;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Domain.ValueObjects;
using VShop.Services.Basket.API.Application.Commands.Shared;

namespace VShop.Services.Basket.API.Application.Commands
{
    public class CreateBasketCommandHandler : ICommandHandler<CreateBasketCommand, Success<Domain.Models.BasketAggregate.Basket>>
    {
        private readonly IEventStoreAggregateRepository<Domain.Models.BasketAggregate.Basket, EntityId> _basketRepository;
        
        public CreateBasketCommandHandler(IEventStoreAggregateRepository<Domain.Models.BasketAggregate.Basket, EntityId> basketRepository)
        {
            _basketRepository = basketRepository;
        }
        
        public async Task<OneOf<Success<Domain.Models.BasketAggregate.Basket>, ApplicationError>> Handle(CreateBasketCommand command, CancellationToken cancellationToken)
        {
            Domain.Models.BasketAggregate.Basket basket = Domain.Models.BasketAggregate.Basket.Create
            (
                EntityId.Create(command.CustomerId),
                command.CustomerDiscount
            );

            foreach (BasketItemDto basketItem in command.BasketItems)
            {
                Option<ApplicationError> errorResult = basket.AddProduct
                (
                    EntityId.Create(basketItem.ProductId),
                    ProductQuantity.Create(basketItem.Quantity),
                    Price.Create(basketItem.UnitPrice)
                );

                if (errorResult.IsSome(out ApplicationError error)) return error;
            }
            
            await _basketRepository.SaveAsync(basket);

            return new Success<Domain.Models.BasketAggregate.Basket>(basket);
        }
    }
}