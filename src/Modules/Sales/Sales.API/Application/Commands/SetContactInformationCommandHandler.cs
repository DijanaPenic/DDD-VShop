using OneOf;
using OneOf.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Infrastructure.Messaging.Commands;
using VShop.SharedKernel.Infrastructure.Messaging.Commands.Publishing;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class SetContactInformationCommandHandler : ICommandHandler<SetContactInformationCommand, Success>
    {
        private readonly IAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;

        public SetContactInformationCommandHandler(IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }
        
        public async Task<OneOf<Success, ApplicationError>> Handle(SetContactInformationCommand command, CancellationToken cancellationToken)
        {
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync
            (
                EntityId.Create(command.ShoppingCartId),
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            if (shoppingCart is null) return NotFoundError.Create("Shopping cart not found.");
            
            Option<ApplicationError> errorResult = shoppingCart.Customer.SetContactInformation
            (
                FullName.Create(command.FirstName, command.MiddleName, command.LastName),
                EmailAddress.Create(command.EmailAddress),
                PhoneNumber.Create(command.PhoneNumber),
                command.Gender
            );
            
            if (errorResult.IsSome(out ApplicationError error)) return error;

            await _shoppingCartRepository.SaveAsync(shoppingCart, cancellationToken);

            return new Success();
        }
    }
    
    public record SetContactInformationCommand : Command<Success>
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