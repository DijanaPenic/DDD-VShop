using System;
using OneOf;
using OneOf.Types;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Application.Commands;
using VShop.SharedKernel.EventStore.Repositories.Contracts;
using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class SetContactInformationCommandHandler : ICommandHandler<SetContactInformationCommand, Success>
    {
        private readonly IEventStoreAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;
        private readonly IEventStoreIntegrationRepository _integrationRepository;
        
        public SetContactInformationCommandHandler
        (
            IEventStoreAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository,
            IEventStoreIntegrationRepository integrationRepository
        )
        {
            _shoppingCartRepository = shoppingCartRepository;
            _integrationRepository = integrationRepository;
        }
        
        public async Task<OneOf<Success, ApplicationError>> Handle(SetContactInformationCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync(EntityId.Create(command.ShoppingCartId));
            if (shoppingCart is null) return NotFoundError.Create("Shopping cart not found.");
            
            Option<ApplicationError> errorResult = shoppingCart.Customer.SetContactInformation
            (
                FullName.Create(command.FirstName, command.MiddleName, command.LastName),
                EmailAddress.Create(command.EmailAddress),
                PhoneNumber.Create(command.PhoneNumber),
                command.Gender
            );
            
            if (errorResult.IsSome(out ApplicationError error)) return error;

            await _shoppingCartRepository.SaveAsync(shoppingCart);

            // TODO - need to remove this code after testing
            await _integrationRepository.SaveAsync
            (
                new OrderPlacedIntegrationEvent
                {
                    OrderId = Guid.Empty
                }
            );

            return new Success();
        }
    }
    
    public record SetContactInformationCommand : ICommand<Success>
    {
        public Guid ShoppingCartId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public GenderType Gender { get; set; }
    }
}