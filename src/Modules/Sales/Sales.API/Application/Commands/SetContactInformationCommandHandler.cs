using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.SharedKernel.Infrastructure.Commands.Publishing.Contracts;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class SetContactInformationCommandHandler : ICommandHandler<SetContactInformationCommand>
    {
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;

        public SetContactInformationCommandHandler(IAggregateStore<ShoppingCart> shoppingCartStore)
            => _shoppingCartStore = shoppingCartStore;

        public async Task<Result> Handle
        (
            SetContactInformationCommand command,
            CancellationToken cancellationToken
        )
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId).Data,
                command.Metadata.MessageId,
                cancellationToken
            );
            if (shoppingCart is null) return Result.NotFoundError("Shopping cart not found.");

            Result<FullName> fullNameResult = FullName.Create(command.FirstName, command.MiddleName,
                command.LastName);
            if (fullNameResult.IsError) return fullNameResult.Error;
            
            Result setContactInformationResult = shoppingCart.Customer.SetContactInformation
            (
                fullNameResult.Data,
                EmailAddress.Create(command.EmailAddress).Data,
                PhoneNumber.Create(command.PhoneNumber).Data,
                command.Gender
            );
            if (setContactInformationResult.IsError) return setContactInformationResult.Error;

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