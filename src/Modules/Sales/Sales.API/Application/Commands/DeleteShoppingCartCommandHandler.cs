using OneOf;
using OneOf.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Application.Commands;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class DeleteShoppingCartCommandHandler : ICommandHandler<DeleteShoppingCartCommand, Success>
    {
        private readonly IAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;
        
        public DeleteShoppingCartCommandHandler(IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }
        
        public async Task<OneOf<Success, ApplicationError>> Handle(DeleteShoppingCartCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId),
                command.MessageId,
                command.CorrelationId
            );
            if (shoppingCart is null) return NotFoundError.Create("Shopping cart not found.");

            Option<ApplicationError> errorResult = shoppingCart.RequestDelete();
            
            if (errorResult.IsSome(out ApplicationError error)) return error;

            await _shoppingCartRepository.SaveAsync(shoppingCart);

            return new Success();
        }
    }
    
    public record DeleteShoppingCartCommand : BaseCommand<Success>
    {
        public Guid ShoppingCartId { get; set; }
    }
}